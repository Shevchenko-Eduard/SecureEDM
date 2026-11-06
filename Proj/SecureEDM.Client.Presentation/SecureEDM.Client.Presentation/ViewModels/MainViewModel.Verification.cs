using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecureEDM.Client.Application.Models;

namespace SecureEDM.Client.Presentation.ViewModels;

public partial class MainViewModel
{
    private bool _suppressVerifySelection;

    public ObservableCollection<DocumentOption> VerifyDocuments { get; } = [];
    public ObservableCollection<SignatureOption> VerifySignatures { get; } = [];

    [ObservableProperty]
    private DocumentOption? _selectedVerifyDocument;

    [ObservableProperty]
    private SignatureOption? _selectedVerifySignature;

    [ObservableProperty]
    private bool _isVerifyEmptyVisible = true;

    [ObservableProperty]
    private string _verifyEmptyText = "Выберите подпись и нажмите «Проверить».";

    [ObservableProperty]
    private bool _isVerifyResultVisible;

    [ObservableProperty]
    private bool _isVerifyOk;

    [ObservableProperty]
    private string _verifyVerdictText = string.Empty;

    [ObservableProperty]
    private bool _verifyAuthorshipOk;

    [ObservableProperty]
    private string _verifyAuthorshipText = string.Empty;

    [ObservableProperty]
    private bool _verifyIntegrityOk;

    [ObservableProperty]
    private string _verifyIntegrityText = string.Empty;

    [ObservableProperty]
    private bool _verifyKeyOk;

    [ObservableProperty]
    private string _verifyKeyText = string.Empty;

    [ObservableProperty]
    private string _verifySignerText = string.Empty;

    [ObservableProperty]
    private string _verifyTimeText = string.Empty;

    [ObservableProperty]
    private string _verifyHashText = string.Empty;

    [ObservableProperty]
    private string _verifyVerdictKind = "bad";

    public string VerifySealRingText => IsVerifyOk
        ? "ЭЛЕКТРОННАЯ ПОДПИСЬ ★ ПРОВЕРЕНА ★ "
        : "ПОДПИСЬ НЕ ПОДТВЕРЖДЕНА ✕ ПРОВЕРКА НЕ ПРОЙДЕНА ✕ ";

    public string VerifySealCenterText => IsVerifyOk ? "ВЕРНО" : "ОТКАЗ";
    public bool VerifyAuthorshipBad => !VerifyAuthorshipOk;
    public bool VerifyIntegrityBad => !VerifyIntegrityOk;
    public bool VerifyKeyBad => !VerifyKeyOk;

    partial void OnIsVerifyOkChanged(bool value)
    {
        OnPropertyChanged(nameof(VerifySealRingText));
        OnPropertyChanged(nameof(VerifySealCenterText));
    }

    partial void OnVerifyAuthorshipOkChanged(bool value) =>
        OnPropertyChanged(nameof(VerifyAuthorshipBad));

    partial void OnVerifyIntegrityOkChanged(bool value) =>
        OnPropertyChanged(nameof(VerifyIntegrityBad));

    partial void OnVerifyKeyOkChanged(bool value) =>
        OnPropertyChanged(nameof(VerifyKeyBad));

    partial void OnSelectedVerifyDocumentChanged(DocumentOption? value)
    {
        if (_suppressVerifySelection) return;
        RefreshVerifySignatures();
    }

    [RelayCommand]
    private async Task VerifySignatureAsync()
    {
        if (SelectedVerifySignature is not { IsReal: true } option)
        {
            IsVerifyResultVisible = false;
            IsVerifyEmptyVisible = true;
            VerifyEmptyText = SelectedVerifyDocument is { IsReal: true }
                ? "Для выбранного документа нет подписей. Сначала подпишите его."
                : "Выберите подпись и нажмите «Проверить».";
            return;
        }

        try
        {
            var result = await _service.VerifySignatureAsync(option.Id);
            ApplyVerificationResult(result);
            RefreshAfterVerification();
        }
        catch (Exception ex)
        {
            IsVerifyResultVisible = false;
            IsVerifyEmptyVisible = true;
            VerifyEmptyText = "Не удалось выполнить проверку: " + ex.Message;
            ShowToast("Ошибка проверки: " + ex.Message, "bad");
        }
    }

    public async Task HandlePackageFileAsync(string path)
    {
        try
        {
            var json = await File.ReadAllTextAsync(path);
            var result = await _service.VerifyPackageAsync(json, Path.GetFileName(path));
            ApplyVerificationResult(result);
            RefreshAfterVerification();
        }
        catch (Exception ex)
        {
            IsVerifyResultVisible = false;
            IsVerifyEmptyVisible = true;
            VerifyEmptyText = "Файл не распознан: " + ex.Message;
            ShowToast("Не удалось разобрать пакет", "bad");
        }
    }

    private void ApplyVerificationResult(VerificationResult result)
    {
        IsVerifyEmptyVisible = false;
        IsVerifyResultVisible = true;
        IsVerifyOk = result.IsValid;
        VerifyVerdictKind = result.VerdictKind;
        VerifyVerdictText = result.VerdictText;

        VerifyAuthorshipOk = result.SignatureOk;
        VerifyAuthorshipText = result.SignatureOk
            ? "Подпись создана владельцем ключа"
            : !result.HashMatch
                ? "Подпись для другой версии текста"
                : "Подпись не соответствует ключу";

        VerifyIntegrityOk = result.HashMatch;
        VerifyIntegrityText = result.HashMatch
            ? "Текст не менялся"
            : "Текст отличается от подписанного";

        VerifyKeyOk = result.KeyOk;
        VerifyKeyText = result.Revoked
            ? "отозван"
            : result.Expired
                ? "истёк"
                : "действует";

        VerifySignerText = $"{result.Owner}, {result.AlgorithmLabel}";
        VerifyTimeText = FormatTime(result.Time);
        VerifyHashText = result.Hash;
    }

    private void RefreshAfterVerification()
    {
        RefreshStats();
        RefreshSignatures();
        _ = RefreshLogAsync();
    }

    private void RefreshVerifySelectors()
    {
        _suppressVerifySelection = true;
        try
        {
            var oldDocumentId = SelectedVerifyDocument?.Id;

            VerifyDocuments.Clear();
            foreach (var document in _service.Documents)
                VerifyDocuments.Add(new DocumentOption(document.Id, $"{document.Number} {document.Title}"));

            if (VerifyDocuments.Count == 0)
                VerifyDocuments.Add(new DocumentOption(string.Empty, "Нет документов"));

            SelectedVerifyDocument =
                VerifyDocuments.FirstOrDefault(d => d.Id == oldDocumentId) ??
                VerifyDocuments.FirstOrDefault(d => d.IsReal);
        }
        finally
        {
            _suppressVerifySelection = false;
        }

        RefreshVerifySignatures();
    }

    private void RefreshVerifySignatures()
    {
        var oldSignatureId = SelectedVerifySignature?.Id;
        VerifySignatures.Clear();

        if (SelectedVerifyDocument is { IsReal: true } documentOption)
        {
            var signatures = _service.Signatures
                .Where(s => s.DocumentId == documentOption.Id)
                .OrderByDescending(s => s.SignedAt);

            foreach (var signature in signatures)
            {
                var key = _service.FindKey(signature.KeyId);
                VerifySignatures.Add(new SignatureOption(
                    signature.Id,
                    $"{FormatTime(signature.SignedAt)}, {key?.Owner ?? "—"}"));
            }
        }

        if (VerifySignatures.Count == 0)
            VerifySignatures.Add(new SignatureOption(string.Empty, "Нет подписей"));

        SelectedVerifySignature =
            VerifySignatures.FirstOrDefault(s => s.Id == oldSignatureId) ??
            VerifySignatures.FirstOrDefault(s => s.IsReal);
    }
}
