using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecureEDM.Client.Application.Models;
using SecureEDM.Client.Application.Services;
using SecureEDM.Common.Domain.Enums;
using SecureEDM.Client.Infrastructure.Cryptography;
using SecureEDM.Client.Infrastructure.Storage;

namespace SecureEDM.Client.Presentation.ViewModels;

/// <summary>Корневая ViewModel клиента: навигация, сессия, тема и обратная связь.</summary>
public partial class MainViewModel : ViewModelBase
{
    private readonly EdoService _service;
    private bool _initialized;
    private bool _suppressUserChange;
    private Func<Task>? _pendingConfirm;

    public MainViewModel()
        : this(new EdoService(new JsonRegistryStore(), new ClientCrypto()))
    {
    }

    public MainViewModel(EdoService service)
    {
        _service = service;
        Templates =
        [
            new("", "Без шаблона", "", "", false),
            new("order", "Приказ", "Приказ № ___",
                "ПРИКАЗ\n\nО назначении ответственного за ведение электронного документооборота\n\nПриказываю:\n1. Назначить ответственным ______________.\n2. Контроль исполнения приказа оставляю за собой.\n",
                true),
            new("contract", "Договор", "Договор № ___",
                "ДОГОВОР № ___\n\nСтороны: ______________ и ______________.\n\n1. Предмет договора\n______________\n\n2. Срок действия\n______________\n",
                true),
            new("request", "Заявление", "Заявление",
                "Директору ______________\nот ______________\n\nЗАЯВЛЕНИЕ\n\nПрошу ______________.\n",
                true),
            new("act", "Акт приёмки", "Акт приёмки № ___",
                "АКТ ПРИЁМКИ № ___\n\nКомиссия установила: ______________.\n\nРезультат: принято / не принято.\n",
                true),
        ];

        KeyAlgorithms = EdoAlgorithms.KeyAlgorithms
            .Select(algorithm => new KeyAlgorithmOption(
                algorithm,
                EdoAlgorithms.KeyLabel(algorithm),
                EdoAlgorithms.KeyDescription(algorithm),
                EdoAlgorithms.DefaultKeyLength(algorithm),
                EdoAlgorithms.KeyLengths(algorithm),
                EdoAlgorithms.SupportsSigning(algorithm)))
            .ToArray();

        PageSizes = [new(10), new(25), new(50)];
        _selectedPageSize = PageSizes[1];
        _selectedKeyAlgorithm =
            KeyAlgorithms.FirstOrDefault(a => a.Algorithm == EnumKpgAlgorithm.RSA) ??
            KeyAlgorithms[0];
        _selectedKeyLength = _selectedKeyAlgorithm.DefaultLength;
        FillKeySignatureAlgorithms();
        _selectedTemplate = Templates[0];
        _selectedUser = _service.Users[0];
        _selectedHashAlgorithm =
            HashAlgorithms.FirstOrDefault(a => a.Algorithm == EnumDigestAlgorithm.SHA_256) ??
            HashAlgorithms[0];
        _selectedHashRevocationReason = RevocationReasons[0];
        _selectedKeyRevocationReason = RevocationReasons[0];
        _selectedSignatureRevocationReason = RevocationReasons[0];
        _selectedConflictStrategy = ConflictStrategies[0];

        _ = InitializeAsync();
    }

    public IReadOnlyList<EdoUser> Users => _service.Users;
    public IReadOnlyList<TemplateOption> Templates { get; }
    public IReadOnlyList<KeyAlgorithmOption> KeyAlgorithms { get; }
    public IReadOnlyList<PageSizeOption> PageSizes { get; }

    public IReadOnlyList<RevocationReasonOption> RevocationReasons { get; } =
    [
        new(SecureEDM.Common.Domain.Enums.EnumRevokedReason.Unspecified, "Не указана"),
        new(SecureEDM.Common.Domain.Enums.EnumRevokedReason.Outdated, "Устарел"),
        new(SecureEDM.Common.Domain.Enums.EnumRevokedReason.Cancelled, "Отменён"),
        new(SecureEDM.Common.Domain.Enums.EnumRevokedReason.Compromise, "Компрометация"),
    ];

    public ObservableCollection<ToastViewModel> Toasts { get; } = [];

    [ObservableProperty]
    private EdoUser? _selectedUser;

    [ObservableProperty]
    private int _selectedTabIndex;

    [ObservableProperty]
    private string _theme = "auto";

    [ObservableProperty]
    private int _documentCount;

    [ObservableProperty]
    private int _keyCount;

    [ObservableProperty]
    private int _signatureCount;

    [ObservableProperty]
    private bool _isConfirmVisible;

    [ObservableProperty]
    private string _confirmTitle = "Подтверждение";

    [ObservableProperty]
    private string _confirmText = string.Empty;

    public bool IsDocumentsTab => SelectedTabIndex == 0;
    public bool IsKeysTab => SelectedTabIndex == 1;
    public bool IsSigningTab => SelectedTabIndex == 2;
    public bool IsVerificationTab => SelectedTabIndex == 3;
    public bool IsHashesTab => SelectedTabIndex == 4;
    public bool IsFilesTab => SelectedTabIndex == 5;
    public bool IsLogTab => SelectedTabIndex == 6;
    public bool IsSettingsTab => SelectedTabIndex == 7;
    public bool IsAboutTab => SelectedTabIndex == 8;

    public bool IsAutoTheme => Theme == "auto";
    public bool IsLightTheme => Theme == "light";
    public bool IsDarkTheme => Theme == "dark";

    public bool CanEditDocuments => SelectedUser?.Role is "author" or "admin";
    public bool CanSign => SelectedUser?.Role is "author" or "admin";
    public bool CanManageKeys => SelectedUser?.Role is "author" or "admin";
    public bool CanManageFiles => SelectedUser?.Role is "author" or "admin";
    public bool CanManageSettings => SelectedUser?.Role is "author" or "admin";
    public bool CanClear => SelectedUser?.Role == "admin";
    public bool CanExport => SelectedUser?.Role == "admin";

    public string SelectedUserRole => SelectedUser?.Title ?? "—";

    public string AboutPackageSample => """
        {
          "format": "edo-signature/2",
          "document": { "number", "title", "author", "content", "sha256" },
          "signature": { "algorithm", "signatureAlgorithm", "sigFormat": "DER", "value", "signedAt" },
          "publicKey": { "owner", "spki", "fingerprint", "validFrom", "validUntil", "status" }
        }
        """;

    private async Task InitializeAsync()
    {
        try
        {
            await _service.InitializeAsync();

            _suppressUserChange = true;
            SelectedUser = Users.FirstOrDefault(u => u.Id == _service.SessionUserId) ?? Users[0];
            _suppressUserChange = false;

            Theme = _service.Theme;
            ApplyTheme(Theme);
            LoadSettingsFromService();
            _initialized = true;
            RefreshAll();
        }
        catch (Exception ex)
        {
            ShowToast("Не удалось загрузить реестр: " + ex.Message, "bad");
        }
    }

    partial void OnSelectedUserChanged(EdoUser? value)
    {
        OnPropertyChanged(nameof(CanEditDocuments));
        OnPropertyChanged(nameof(CanSign));
        OnPropertyChanged(nameof(CanManageKeys));
        OnPropertyChanged(nameof(CanManageFiles));
        OnPropertyChanged(nameof(CanManageSettings));
        OnPropertyChanged(nameof(CanSaveSettings));
        OnPropertyChanged(nameof(CanClear));
        OnPropertyChanged(nameof(CanExport));
        OnPropertyChanged(nameof(CanSignNow));
        OnPropertyChanged(nameof(CanGenerateKey));
        OnPropertyChanged(nameof(SelectedUserRole));

        if (_initialized && !_suppressUserChange && value is not null)
            _ = ChangeUserAsync(value);
    }

    private async Task ChangeUserAsync(EdoUser user)
    {
        try
        {
            await _service.SetCurrentUserAsync(user.Id);
            await RefreshLogAsync();
            RefreshKeys();
            RefreshSelectedKey();
            RefreshDocuments();
        }
        catch (Exception ex)
        {
            ShowToast("Не удалось сменить пользователя: " + ex.Message, "bad");
        }
    }

    partial void OnSelectedTabIndexChanged(int value)
    {
        OnPropertyChanged(nameof(IsDocumentsTab));
        OnPropertyChanged(nameof(IsKeysTab));
        OnPropertyChanged(nameof(IsSigningTab));
        OnPropertyChanged(nameof(IsVerificationTab));
        OnPropertyChanged(nameof(IsHashesTab));
        OnPropertyChanged(nameof(IsFilesTab));
        OnPropertyChanged(nameof(IsLogTab));
        OnPropertyChanged(nameof(IsSettingsTab));
        OnPropertyChanged(nameof(IsAboutTab));

        if (value == 2)
            _ = RefreshSignHashAsync();
    }

    [RelayCommand]
    private void SelectTab(string? tab)
    {
        SelectedTabIndex = tab switch
        {
            "docs" => 0,
            "keys" => 1,
            "sign" => 2,
            "verify" => 3,
            "hashes" => 4,
            "files" => 5,
            "log" => 6,
            "settings" => 7,
            "about" => 8,
            _ => SelectedTabIndex,
        };
    }

    partial void OnThemeChanged(string value)
    {
        OnPropertyChanged(nameof(IsAutoTheme));
        OnPropertyChanged(nameof(IsLightTheme));
        OnPropertyChanged(nameof(IsDarkTheme));
        ApplyTheme(value);
    }

    [RelayCommand]
    private async Task SetThemeAsync(string? theme)
    {
        if (theme is not ("auto" or "light" or "dark")) return;
        Theme = theme;
        try
        {
            await _service.SetThemeAsync(theme);
        }
        catch
        {
            // Смена темы не должна ломать работу приложения.
        }
    }

    private static void ApplyTheme(string theme)
    {
        if (Avalonia.Application.Current is not { } app) return;

        app.RequestedThemeVariant = theme switch
        {
            "light" => ThemeVariant.Light,
            "dark" => ThemeVariant.Dark,
            _ => ThemeVariant.Default,
        };
    }

    public void ShowToast(string message, string kind = "")
    {
        var toast = new ToastViewModel(message, kind);
        Toasts.Add(toast);
        _ = DismissToastAsync(toast);
    }

    private async Task DismissToastAsync(ToastViewModel toast)
    {
        await Task.Delay(3200);
        Toasts.Remove(toast);
    }

    public void ShowConfirm(string title, string text, Func<Task> action)
    {
        _pendingConfirm = action;
        ConfirmTitle = title;
        ConfirmText = text;
        IsConfirmVisible = true;
    }

    [RelayCommand]
    private async Task ConfirmAsync()
    {
        var action = _pendingConfirm;
        _pendingConfirm = null;
        IsConfirmVisible = false;

        if (action is null) return;
        try
        {
            await action();
        }
        catch (Exception ex)
        {
            ShowToast(ex.Message, "bad");
        }
    }

    [RelayCommand]
    private void CancelConfirm()
    {
        _pendingConfirm = null;
        IsConfirmVisible = false;
    }

    private void RefreshAll()
    {
        RefreshStats();
        RefreshDocuments();
        RefreshKeys();
        RefreshSignSelectors();
        RefreshVerifySelectors();
        RefreshHashSelectors();
        RefreshFiles();
        RefreshSignatures();
        RefreshDocumentHistory();
        _ = RefreshLogAsync();
    }

    private void RefreshStats()
    {
        DocumentCount = _service.Documents.Count;
        KeyCount = _service.Keys.Count;
        SignatureCount = _service.Signatures.Count;
    }
}
