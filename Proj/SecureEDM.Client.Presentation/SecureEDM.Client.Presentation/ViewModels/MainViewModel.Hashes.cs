using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecureEDM.Client.Application.Models;
using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Client.Presentation.ViewModels;

public partial class MainViewModel
{
    public IReadOnlyList<HashAlgorithmOption> HashAlgorithms { get; } =
        Enum.GetValues<EnumDigestAlgorithm>()
            .Select(algorithm => new HashAlgorithmOption(
                algorithm, EdoLabels.HashAlgorithm(algorithm)))
            .ToArray();

    public ObservableCollection<DocumentOption> HashDocuments { get; } = [];
    public ObservableCollection<HashRow> Hashes { get; } = [];

    [ObservableProperty]
    private DocumentOption? _selectedHashDocument;

    [ObservableProperty]
    private HashAlgorithmOption? _selectedHashAlgorithm;

    [ObservableProperty]
    private RevocationReasonOption? _selectedHashRevocationReason;

    [ObservableProperty]
    private HashRow? _selectedHashRow;

    [ObservableProperty]
    private bool _hasHashes;

    [ObservableProperty]
    private string _emptyHashesText = "У документа пока нет зарегистрированных хешей.";

    partial void OnSelectedHashDocumentChanged(DocumentOption? value) => RefreshHashes();

    [RelayCommand]
    private async Task AddHashAsync()
    {
        if (!CanEditDocuments)
        {
            ShowToast("Недостаточно прав", "bad");
            return;
        }

        if (SelectedHashDocument is not { IsReal: true } document ||
            SelectedHashAlgorithm is null)
        {
            ShowToast("Выберите документ и алгоритм", "bad");
            return;
        }

        try
        {
            await _service.AddHashAsync(document.Id, SelectedHashAlgorithm.Algorithm);
            ShowToast($"Хеш {SelectedHashAlgorithm.Name} добавлен", "ok");
            RefreshAll();
        }
        catch (Exception ex)
        {
            ShowToast("Не удалось добавить хеш: " + ex.Message, "bad");
        }
    }

    [RelayCommand]
    private async Task SetPrimaryHashAsync(string? id)
    {
        if (!CanEditDocuments)
        {
            ShowToast("Недостаточно прав", "bad");
            return;
        }

        try
        {
            await _service.SetPrimaryHashAsync(id ?? string.Empty);
            ShowToast("Основной хеш изменён", "ok");
            RefreshAll();
        }
        catch (Exception ex)
        {
            ShowToast("Не удалось изменить основной хеш: " + ex.Message, "bad");
        }
    }

    [RelayCommand]
    private void RevokeHash(string? id)
    {
        if (!CanEditDocuments) return;

        var hash = _service.FindHash(id);
        if (hash is null || hash.RevokedAt is not null) return;

        var reason = SelectedHashRevocationReason?.Reason ?? EnumRevokedReason.Unspecified;
        ShowConfirm(
            "Отозвать хеш?",
            $"Хеш {ShortHash(hash.HashHex)} будет помечен как отозванный ({EdoLabels.RevokedReason(reason)}).",
            async () =>
            {
                await _service.RevokeHashAsync(hash.Id, reason);
                ShowToast("Хеш отозван", "warn");
                RefreshAll();
            });
    }

    [RelayCommand]
    private void DeleteHash(string? id)
    {
        if (!CanEditDocuments) return;

        var hash = _service.FindHash(id);
        if (hash is null) return;

        ShowConfirm(
            "Удалить хеш?",
            $"Запись хеша {ShortHash(hash.HashHex)} будет удалена из реестра.",
            async () =>
            {
                await _service.DeleteHashAsync(hash.Id);
                ShowToast("Хеш удалён", "warn");
                RefreshAll();
            });
    }

    private void RefreshHashSelectors()
    {
        var oldDocumentId = SelectedHashDocument?.Id;

        HashDocuments.Clear();
        foreach (var document in _service.Documents)
            HashDocuments.Add(new DocumentOption(document.Id, $"{document.Number} {document.Title}"));

        if (HashDocuments.Count == 0)
            HashDocuments.Add(new DocumentOption(string.Empty, "Нет документов"));

        SelectedHashDocument =
            HashDocuments.FirstOrDefault(d => d.Id == oldDocumentId) ??
            HashDocuments.FirstOrDefault(d => d.IsReal);

        RefreshHashes();
    }

    private void RefreshHashes()
    {
        var oldHashId = SelectedHashRow?.Id;
        Hashes.Clear();

        if (SelectedHashDocument is { IsReal: true } document)
        {
            foreach (var hash in _service.Hashes
                         .Where(h => h.DocumentId == document.Id)
                         .OrderByDescending(h => h.CreatedAt))
            {
                var revoked = hash.RevokedAt is not null;
                var statusText = revoked
                    ? "отозван"
                    : hash.IsPrimary
                        ? "основной"
                        : "доп.";
                var statusKind = revoked ? "bad" : hash.IsPrimary ? "ok" : "warn";
                var statusFull = revoked
                    ? $"Отозван: {EdoLabels.RevokedReason(hash.RevokedReason)}"
                    : hash.IsPrimary
                        ? "основной"
                        : "дополнительный";

                Hashes.Add(new HashRow(
                    hash.Id,
                    EdoLabels.HashAlgorithm(hash.Algorithm),
                    ShortHash(hash.HashHex, 10),
                    hash.HashHex,
                    hash.IsPrimary,
                    statusKind,
                    statusText,
                    statusFull,
                    FormatShortTime(hash.CreatedAt),
                    hash.Id == oldHashId));
            }
        }

        HasHashes = Hashes.Count > 0;
        EmptyHashesText = SelectedHashDocument is { IsReal: true }
            ? "У документа пока нет зарегистрированных хешей."
            : "Документов пока нет. Создайте документ на вкладке «Документы».";

        SelectedHashRow =
            Hashes.FirstOrDefault(h => h.Id == oldHashId) ??
            Hashes.FirstOrDefault();
    }
}
