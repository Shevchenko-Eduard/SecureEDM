using SecureEDM.Common.Domain.Enums;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecureEDM.Client.Application.Models;

namespace SecureEDM.Client.Presentation.ViewModels;

public partial class MainViewModel
{
    private bool _suppressSignSelection;
    private string? _lastPackageJson;
    private string _lastPackageFileName = "signature.json";

    public ObservableCollection<DocumentOption> SignDocuments { get; } = [];
    public ObservableCollection<KeyOption> SignKeys { get; } = [];
    public ObservableCollection<SignatureAlgorithmOption> SignSignatureAlgorithms { get; } = [];

    [ObservableProperty]
    private DocumentOption? _selectedSignDocument;

    [ObservableProperty]
    private KeyOption? _selectedSignKey;

    [ObservableProperty]
    private SignatureAlgorithmOption? _selectedSignAlgorithm;

    [ObservableProperty]
    private string _signHash = "—";

    [ObservableProperty]
    private bool _hasSignResult;

    [ObservableProperty]
    private bool _isSignResultOk = true;

    [ObservableProperty]
    private string _signResultOwner = "—";

    [ObservableProperty]
    private string _signResultAlgorithm = "—";

    [ObservableProperty]
    private string _signResultTime = "—";

    [ObservableProperty]
    private string _signResultSignaturePreview = string.Empty;

    public string SignaturePackageFileName => _lastPackageFileName;

    public bool CanSignNow =>
        CanSign &&
        SelectedSignDocument is { IsReal: true } &&
        SelectedSignKey is { IsReal: true } &&
        SelectedSignAlgorithm is not null;

    public bool HasSignSignatureAlgorithms => SignSignatureAlgorithms.Count > 0;

    partial void OnSelectedSignDocumentChanged(DocumentOption? value)
    {
        if (_suppressSignSelection) return;
        OnPropertyChanged(nameof(CanSignNow));
        _ = RefreshSignHashAsync();
    }

    partial void OnSelectedSignKeyChanged(KeyOption? value)
    {
        if (_suppressSignSelection) return;
        FillSignSignatureAlgorithms();
        OnPropertyChanged(nameof(CanSignNow));
    }

    partial void OnSelectedSignAlgorithmChanged(SignatureAlgorithmOption? value) =>
        OnPropertyChanged(nameof(CanSignNow));

    [RelayCommand]
    private async Task SignDocumentAsync()
    {
        if (!CanSign)
        {
            ShowToast("Недостаточно прав для подписи", "bad");
            return;
        }

        if (SelectedSignDocument is not { IsReal: true } documentOption ||
            SelectedSignKey is not { IsReal: true } keyOption ||
            SelectedSignAlgorithm is not { } signatureOption)
            return;

        try
        {
            var signature = await _service.SignDocumentAsync(
                documentOption.Id, keyOption.Id, signatureOption.Algorithm);
            _lastPackageJson = _service.BuildSignaturePackageJson(signature.Id);

            var document = _service.FindDocument(documentOption.Id);
            var key = _service.FindKey(keyOption.Id);
            _lastPackageFileName = (document?.Number ?? "document") + "-signature.json";

            SignResultOwner = key?.Owner ?? "—";
            SignResultAlgorithm = EdoAlgorithms.SignatureAlgorithmLabel(signature);
            SignResultTime = FormatTime(signature.SignedAt);
            SignResultSignaturePreview = signature.SignatureValue.Length > 120
                ? signature.SignatureValue[..120] + "…"
                : signature.SignatureValue;
            IsSignResultOk = true;
            HasSignResult = true;

            ShowToast("Документ подписан", "ok");
            RefreshAll();
        }
        catch (Exception ex)
        {
            ShowToast("Ошибка подписи: " + ex.Message, "bad");
        }
    }

    public string BuildSignaturePackageJson() => _lastPackageJson ?? string.Empty;

    private void RefreshSignSelectors()
    {
        _suppressSignSelection = true;
        try
        {
            var oldDocumentId = SelectedSignDocument?.Id;
            var oldKeyId = SelectedSignKey?.Id;
            var oldAlgorithm = SelectedSignAlgorithm?.Algorithm;

            SignDocuments.Clear();
            foreach (var document in _service.Documents)
                SignDocuments.Add(new DocumentOption(document.Id, $"{document.Number} {document.Title}"));

            if (SignDocuments.Count == 0)
                SignDocuments.Add(new DocumentOption(string.Empty, "Нет документов"));

            SignKeys.Clear();
            foreach (var key in _service.Keys.Where(k =>
                         _service.KeyIsActive(k) &&
                         _service.HasPrivateKey(k.Id) &&
                         _service.GetCompatibleSignatureAlgorithms(k).Count > 0))
                SignKeys.Add(new KeyOption(
                    key.Id,
                    $"{key.Owner}, {EdoAlgorithms.KeyAlgorithmLabel(key)}"));

            if (SignKeys.Count == 0)
                SignKeys.Add(new KeyOption(string.Empty, "Нет ключей с поддержкой подписи"));

            SelectedSignDocument =
                SignDocuments.FirstOrDefault(d => d.Id == oldDocumentId) ??
                SignDocuments.FirstOrDefault(d => d.IsReal);

            SelectedSignKey =
                SignKeys.FirstOrDefault(k => k.Id == oldKeyId) ??
                SignKeys.FirstOrDefault(k => k.IsReal);

            FillSignSignatureAlgorithms(oldAlgorithm);
        }
        finally
        {
            _suppressSignSelection = false;
        }

        OnPropertyChanged(nameof(CanSignNow));
        _ = RefreshSignHashAsync();
    }

    private void FillSignSignatureAlgorithms(EnumSignedAlgorithm? preferredAlgorithm = null)
    {
        SignSignatureAlgorithms.Clear();
        var key = _service.FindKey(SelectedSignKey?.Id);

        if (key is not null)
        {
            foreach (var algorithm in _service.GetCompatibleSignatureAlgorithms(key))
                SignSignatureAlgorithms.Add(new SignatureAlgorithmOption(
                    algorithm, EdoAlgorithms.SignatureLabel(algorithm)));
        }

        var preferred = preferredAlgorithm ?? (key is null ? null : _service.GetDefaultSignatureAlgorithm(key));
        SelectedSignAlgorithm =
            SignSignatureAlgorithms.FirstOrDefault(a => a.Algorithm == preferred) ??
            SignSignatureAlgorithms.FirstOrDefault();

        OnPropertyChanged(nameof(HasSignSignatureAlgorithms));
        OnPropertyChanged(nameof(CanSignNow));
    }

    private async Task RefreshSignHashAsync()
    {
        if (SelectedSignDocument is not { IsReal: true } option)
        {
            SignHash = "—";
            return;
        }

        var document = _service.FindDocument(option.Id);
        if (document is null)
        {
            SignHash = "—";
            return;
        }

        try
        {
            var hash = await _service.GetDocumentHashAsync(document);
            if (SelectedSignDocument?.Id == option.Id) SignHash = hash;
        }
        catch
        {
            if (SelectedSignDocument?.Id == option.Id) SignHash = "—";
        }
    }
}
