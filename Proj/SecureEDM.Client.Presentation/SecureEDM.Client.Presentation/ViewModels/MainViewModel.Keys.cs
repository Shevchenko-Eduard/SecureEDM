using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecureEDM.Client.Application.Models;
using SecureEDM.Common.Domain.Enums;

namespace SecureEDM.Client.Presentation.ViewModels;

public partial class MainViewModel
{
    public ObservableCollection<KeyRow> Keys { get; } = [];

    [ObservableProperty]
    private string? _keyOwner;

    [ObservableProperty]
    private string? _keyOwnerError;

    [ObservableProperty]
    private KeyAlgorithmOption? _selectedKeyAlgorithm;

    [ObservableProperty]
    private int _selectedKeyLength;

    [ObservableProperty]
    private SignatureAlgorithmOption? _selectedKeySignatureAlgorithm;

    [ObservableProperty]
    private string _keyDaysText = "365";

    [ObservableProperty]
    private string? _keyDaysError;

    [ObservableProperty]
    private string? _keySearch;

    [ObservableProperty]
    private string? _selectedKeyId;

    [ObservableProperty]
    private string _selectedKeyPem = string.Empty;

    [ObservableProperty]
    private bool _isKeyPemVisible;

    [ObservableProperty]
    private bool _canRevokeSelectedKey;

    [ObservableProperty]
    private RevocationReasonOption? _selectedKeyRevocationReason;

    [ObservableProperty]
    private string? _keyAttestationInput;

    [ObservableProperty]
    private string? _keyAttestationError;

    [ObservableProperty]
    private string? _keyRemoteOwnerIdText;

    [ObservableProperty]
    private string _keyAttestationStatus = "отсутствует";

    [ObservableProperty]
    private bool _canAttestSelectedKey;

    [ObservableProperty]
    private bool _isGeneratingKey;

    public ObservableCollection<SignatureAlgorithmOption> KeySignatureAlgorithms { get; } = [];
    public IReadOnlyList<int> KeyLengths => SelectedKeyAlgorithm?.Lengths ?? [];

    [ObservableProperty]
    private bool _hasKeyRows;

    [ObservableProperty]
    private string _emptyKeysText = "Ключей пока нет. Сгенерируйте пару слева.";

    public bool HasKeyOwnerError => !string.IsNullOrEmpty(KeyOwnerError);
    public bool HasKeyDaysError => !string.IsNullOrEmpty(KeyDaysError);
    public bool HasKeyAttestationError => !string.IsNullOrEmpty(KeyAttestationError);
    public bool HasKeySignatureAlgorithms => KeySignatureAlgorithms.Count > 0;

    partial void OnSelectedKeyAlgorithmChanged(KeyAlgorithmOption? value)
    {
        if (value is null) return;
        SelectedKeyLength = value.DefaultLength;
        OnPropertyChanged(nameof(KeyLengths));
        OnPropertyChanged(nameof(HasKeySignatureAlgorithms));
        FillKeySignatureAlgorithms();
    }

    partial void OnSelectedKeySignatureAlgorithmChanged(SignatureAlgorithmOption? value)
    {
        // Selection is intentionally kept; the chosen algorithm is used as the key default.
    }

    private void FillKeySignatureAlgorithms()
    {
        KeySignatureAlgorithms.Clear();
        if (SelectedKeyAlgorithm is not { } option) return;

        var preferred = EdoAlgorithms.DefaultSignatureAlgorithm(option.Algorithm);
        foreach (var algorithm in EdoAlgorithms.CompatibleSignatures(option.Algorithm))
            KeySignatureAlgorithms.Add(new SignatureAlgorithmOption(
                algorithm, EdoAlgorithms.SignatureLabel(algorithm)));

        SelectedKeySignatureAlgorithm =
            KeySignatureAlgorithms.FirstOrDefault(a => a.Algorithm == preferred) ??
            KeySignatureAlgorithms.FirstOrDefault();
        OnPropertyChanged(nameof(HasKeySignatureAlgorithms));
    }
    public string GenerateKeyButtonText => IsGeneratingKey ? "Генерация…" : "Сгенерировать";
    public bool CanGenerateKey => CanManageKeys && !IsGeneratingKey;

    partial void OnKeyOwnerErrorChanged(string? value) =>
        OnPropertyChanged(nameof(HasKeyOwnerError));

    partial void OnKeyDaysErrorChanged(string? value) =>
        OnPropertyChanged(nameof(HasKeyDaysError));

    partial void OnKeyAttestationErrorChanged(string? value) =>
        OnPropertyChanged(nameof(HasKeyAttestationError));

    partial void OnIsGeneratingKeyChanged(bool value)
    {
        OnPropertyChanged(nameof(GenerateKeyButtonText));
        OnPropertyChanged(nameof(CanGenerateKey));
    }

    partial void OnKeySearchChanged(string? value) => RefreshKeys();

    [RelayCommand]
    private async Task GenerateKeyAsync()
    {
        if (!CanManageKeys)
        {
            ShowToast("Недостаточно прав", "bad");
            return;
        }

        var owner = (KeyOwner ?? string.Empty).Trim();
        KeyOwnerError = string.IsNullOrWhiteSpace(owner) ? "Укажите владельца" : null;

        if (!int.TryParse(KeyDaysText, out var days) || days is < 1 or > 3650)
            KeyDaysError = "Укажите число от 1 до 3650";
        else
            KeyDaysError = null;

        if (KeyOwnerError is not null || KeyDaysError is not null) return;

        var algorithm = SelectedKeyAlgorithm?.Algorithm ?? EnumKpgAlgorithm.RSA;

        IsGeneratingKey = true;
        try
        {
            await _service.GenerateKeyAsync(
                owner,
                algorithm,
                SelectedKeyLength,
                SelectedKeySignatureAlgorithm?.Algorithm,
                days);
            ShowToast("Ключевая пара создана", "ok");
            RefreshAll();
        }
        catch (Exception ex)
        {
            ShowToast("Не удалось создать ключ: " + ex.Message, "bad");
        }
        finally
        {
            IsGeneratingKey = false;
        }
    }

    [RelayCommand]
    private void SelectKey(string? id)
    {
        var key = _service.FindKey(id);
        if (key is null) return;

        SelectedKeyId = key.Id;
        IsKeyPemVisible = true;
        RefreshSelectedKey();
        RefreshKeys();
    }

    [RelayCommand]
    private void RevokeSelectedKey()
    {
        if (!CanManageKeys) return;

        var key = _service.FindKey(SelectedKeyId);
        if (key is null || key.Status != EdoKeyStatus.Active) return;

        var reason = SelectedKeyRevocationReason?.Reason ?? EnumRevokedReason.Unspecified;
        ShowConfirm(
            "Отозвать ключ?",
            $"Ключ «{key.Owner}» будет помечен как отозванный ({EdoLabels.RevokedReason(reason)}). Все подписи, сделанные этим ключом, станут недействительными.",
            async () =>
            {
                await _service.RevokeKeyAsync(key.Id, reason);
                ShowToast("Ключ отозван", "warn");
                RefreshAll();
                RefreshSelectedKey();
            });
    }

    private void RefreshSelectedKey()
    {
        var key = _service.FindKey(SelectedKeyId);
        if (key is null)
        {
            IsKeyPemVisible = false;
            CanRevokeSelectedKey = false;
            SelectedKeyPem = string.Empty;
            return;
        }

        SelectedKeyPem = _service.GetPublicKeyPem(key.Id);
        IsKeyPemVisible = true;
        CanRevokeSelectedKey = CanManageKeys && key.Status == EdoKeyStatus.Active;
        CanAttestSelectedKey = CanManageKeys && key.Status == EdoKeyStatus.Active && string.IsNullOrEmpty(key.AttestationBase64);
        KeyAttestationStatus = string.IsNullOrEmpty(key.AttestationBase64)
            ? "отсутствует"
            : "есть" + (key.AttestedAt is { } attestedAt ? $", {FormatTime(attestedAt)}" : string.Empty);
        KeyAttestationInput = string.Empty;
        KeyAttestationError = null;
        KeyRemoteOwnerIdText = key.RemoteOwnerId?.ToString() ?? string.Empty;
    }

    [RelayCommand]
    private void DeleteSelectedKey()
    {
        if (!CanManageKeys) return;

        var key = _service.FindKey(SelectedKeyId);
        if (key is null) return;

        ShowConfirm(
            "Удалить ключ?",
            $"Ключ «{key.Owner}» будет полностью удалён из реестра. Подписи, ссылающиеся на него, станут недействительными.",
            async () =>
            {
                await _service.DeleteKeyAsync(key.Id);
                SelectedKeyId = null;
                IsKeyPemVisible = false;
                SelectedKeyPem = string.Empty;
                CanRevokeSelectedKey = false;
                CanAttestSelectedKey = false;
                ShowToast("Ключ удалён", "warn");
                RefreshAll();
            });
    }

    [RelayCommand]
    private async Task AttestSelectedKeyAsync()
    {
        if (!CanManageKeys)
        {
            ShowToast("Недостаточно прав", "bad");
            return;
        }

        var key = _service.FindKey(SelectedKeyId);
        if (key is null)
        {
            ShowToast("Ключ не выбран", "bad");
            return;
        }

        int? remoteOwnerId = null;
        if (!string.IsNullOrWhiteSpace(KeyRemoteOwnerIdText))
        {
            if (!int.TryParse(KeyRemoteOwnerIdText, out var parsed) || parsed <= 0)
            {
                KeyAttestationError = "Remote owner ID должен быть положительным числом.";
                return;
            }
            remoteOwnerId = parsed;
        }

        if (string.IsNullOrWhiteSpace(KeyAttestationInput))
        {
            KeyAttestationError = "Укажите Base64-строку аттестации.";
            return;
        }

        KeyAttestationError = null;
        try
        {
            await _service.AttestKeyAsync(key.Id, KeyAttestationInput, remoteOwnerId);
            ShowToast("Ключ аттестован", "ok");
            RefreshAll();
            RefreshSelectedKey();
        }
        catch (Exception ex)
        {
            KeyAttestationError = ex.Message;
            ShowToast("Не удалось аттестовать ключ: " + ex.Message, "bad");
        }
    }

    private void RefreshKeys()
    {
        IEnumerable<EdoKey> rows = _service.Keys;

        if (!string.IsNullOrWhiteSpace(KeySearch))
        {
            var query = KeySearch.Trim();
            rows = rows.Where(k =>
                k.Owner.Contains(query, StringComparison.CurrentCultureIgnoreCase) ||
                k.Fingerprint.Contains(query, StringComparison.CurrentCultureIgnoreCase));
        }

        var now = DateTimeOffset.Now;
        Keys.Clear();
        foreach (var key in rows.OrderByDescending(k => k.CreatedAt))
        {
            var expired = key.Status != EdoKeyStatus.Revoked && key.ValidUntil <= now;
            var revoked = key.Status == EdoKeyStatus.Revoked;
            var active = !expired && !revoked;

            Keys.Add(new KeyRow(
                key.Id,
                key.Owner,
                EdoAlgorithms.KeyAlgorithmLabel(key),
                FormatShortDate(key.ValidUntil),
                FormatTime(key.ValidUntil),
                ShortHash(key.Fingerprint, 10),
                key.Fingerprint,
                revoked ? "отозван" : expired ? "истёк" : "активен",
                active,
                expired,
                revoked,
                _service.HasPrivateKey(key.Id),
                key.Id == SelectedKeyId));
        }

        HasKeyRows = Keys.Count > 0;
        EmptyKeysText = string.IsNullOrWhiteSpace(KeySearch)
            ? "Ключей пока нет. Сгенерируйте пару слева."
            : "Ничего не найдено.";

        if (SelectedKeyId is not null && _service.FindKey(SelectedKeyId) is null)
        {
            SelectedKeyId = null;
            IsKeyPemVisible = false;
            SelectedKeyPem = string.Empty;
        }
    }
}
