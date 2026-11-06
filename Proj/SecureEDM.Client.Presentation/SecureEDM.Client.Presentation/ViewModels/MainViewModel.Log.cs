using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecureEDM.Client.Application.Models;

namespace SecureEDM.Client.Presentation.ViewModels;

public partial class MainViewModel
{
    public ObservableCollection<SignatureRow> Signatures { get; } = [];
    public ObservableCollection<LogRow> LogEntries { get; } = [];

    [ObservableProperty]
    private bool _isLogMacOk = true;

    [ObservableProperty]
    private bool _hasLogEntries;

    [ObservableProperty]
    private int _logEventCount;

    [ObservableProperty]
    private RevocationReasonOption? _selectedSignatureRevocationReason;

    public string LogMacLabel => IsLogMacOk ? "MAC ок" : "MAC не сходится";
    public bool IsLogMacBad => !IsLogMacOk;
    public bool HasSignatures => Signatures.Count > 0;

    partial void OnIsLogMacOkChanged(bool value)
    {
        OnPropertyChanged(nameof(LogMacLabel));
        OnPropertyChanged(nameof(IsLogMacBad));
    }

    [RelayCommand]
    private void RevokeSignature(string? id)
    {
        if (!CanSign) return;

        var signature = _service.FindSignature(id);
        if (signature is null || signature.RevokedAt is not null) return;

        var reason = SelectedSignatureRevocationReason?.Reason ??
                     SecureEDM.Common.Domain.Enums.EnumRevokedReason.Unspecified;
        ShowConfirm(
            "Отозвать подпись?",
            $"Подпись будет помечена как отозванная ({EdoLabels.RevokedReason(reason)}).",
            async () =>
            {
                await _service.RevokeSignatureAsync(signature.Id, reason);
                ShowToast("Подпись отозвана", "warn");
                RefreshAll();
            });
    }

    [RelayCommand]
    private void DeleteSignature(string? id)
    {
        if (!CanSign) return;

        var signature = _service.FindSignature(id);
        if (signature is null) return;

        ShowConfirm(
            "Удалить подпись?",
            "Запись подписи будет удалена из реестра. Документ останется без этой подписи.",
            async () =>
            {
                await _service.DeleteSignatureAsync(signature.Id);
                ShowToast("Подпись удалена", "warn");
                RefreshAll();
            });
    }

    [RelayCommand]
    private void ClearAll()
    {
        if (!CanClear)
        {
            ShowToast("Недостаточно прав", "bad");
            return;
        }

        ShowConfirm(
            "Очистить все данные?",
            "Будут удалены документы, ключи, подписи, хеши, отслеживаемые файлы и журналы. Настройки сохранятся. Действие необратимо.",
            async () =>
            {
                await _service.ClearAllAsync();

                _bodyHashVersion++;
                CurrentDocumentId = null;
                DocumentTitle = string.Empty;
                DocumentAuthor = string.Empty;
                DocumentBody = string.Empty;
                SelectedTemplate = Templates[0];
                DocumentFormTitle = "Новый документ";
                DocumentBodyHash = "—";
                DocumentTitleError = null;
                DocumentBodyError = null;

                SelectedKeyId = null;
                SelectedKeyPem = string.Empty;
                IsKeyPemVisible = false;
                CanRevokeSelectedKey = false;

                HasSignResult = false;
                _lastPackageJson = null;

                IsVerifyResultVisible = false;
                IsVerifyEmptyVisible = true;
                VerifyEmptyText = "Выберите подпись и нажмите «Проверить».";

                RefreshAll();
                ShowToast("Данные очищены", "warn");
            });
    }

    public string ExportRegistryJson() => _service.ExportRegistryJson();

    private void RefreshSignatures()
    {
        Signatures.Clear();
        foreach (var signature in _service.Signatures.OrderByDescending(s => s.SignedAt))
        {
            var document = _service.FindDocument(signature.DocumentId);
            var key = _service.FindKey(signature.KeyId);
            var ok = document is not null && key is not null &&
                     string.Equals(signature.Hash, document.Hash, StringComparison.OrdinalIgnoreCase) &&
                     key.Status != Application.Models.EdoKeyStatus.Revoked;

            var revoked = signature.RevokedAt is not null;
            var status = revoked
                ? "отозвана"
                : ok
                    ? "действительна"
                    : key is not null && key.Status == Application.Models.EdoKeyStatus.Revoked
                        ? "ключ отозван"
                        : "недействительна";
            var statusFull = revoked
                ? $"Отозвана: {EdoLabels.RevokedReason(signature.RevokedReason)}"
                : status;

            Signatures.Add(new SignatureRow(
                signature.Id,
                FormatTime(signature.SignedAt),
                document?.Title ?? "(удалён)",
                key?.Owner ?? "—",
                EdoAlgorithms.SignatureAlgorithmLabel(signature),
                ShortHash(signature.Hash, 12),
                signature.Hash,
                status,
                statusFull,
                ok && !revoked,
                revoked));
        }

        OnPropertyChanged(nameof(HasSignatures));
    }

    private async Task RefreshLogAsync()
    {
        IsLogMacOk = _service.VerifyLogMac();
        LogEventCount = _service.Log.Count;

        LogEntries.Clear();
        foreach (var entry in _service.Log)
            LogEntries.Add(new LogRow(
                FormatTime(entry.Time),
                entry.User,
                entry.Type,
                entry.Message));

        HasLogEntries = LogEntries.Count > 0;
        await Task.CompletedTask;
    }
}
