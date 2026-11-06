using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecureEDM.Client.Application.Models;
using SecureEDM.Client.Domain.Enums;

namespace SecureEDM.Client.Presentation.ViewModels;

public partial class MainViewModel
{
    public IReadOnlyList<ConflictStrategyOption> ConflictStrategies { get; } =
    [
        new(EnumConflictStrategy.KeepLocal, EdoLabels.ConflictStrategy(EnumConflictStrategy.KeepLocal)),
        new(EnumConflictStrategy.KeepRemote, EdoLabels.ConflictStrategy(EnumConflictStrategy.KeepRemote)),
        new(EnumConflictStrategy.KeepRemoteInNewLocal, EdoLabels.ConflictStrategy(EnumConflictStrategy.KeepRemoteInNewLocal)),
        new(EnumConflictStrategy.KeepLocalInNewRemote, EdoLabels.ConflictStrategy(EnumConflictStrategy.KeepLocalInNewRemote)),
    ];

    [ObservableProperty]
    private string _settingsAuthApiUrl = string.Empty;

    [ObservableProperty]
    private string _settingsBridgeApiUrl = string.Empty;

    [ObservableProperty]
    private string _settingsRootPath = string.Empty;

    [ObservableProperty]
    private string _settingsSyncIntervalMinutes = "5";

    [ObservableProperty]
    private ConflictStrategyOption? _selectedConflictStrategy;

    [ObservableProperty]
    private string? _settingsError;

    [ObservableProperty]
    private bool _isSavingSettings;

    public bool HasSettingsError => !string.IsNullOrEmpty(SettingsError);
    public bool CanSaveSettings => CanManageSettings && !IsSavingSettings;
    public string SettingsSyncIntervalHint => "Интервал указывается в минутах.";

    partial void OnSettingsErrorChanged(string? value) =>
        OnPropertyChanged(nameof(HasSettingsError));

    partial void OnIsSavingSettingsChanged(bool value) =>
        OnPropertyChanged(nameof(CanSaveSettings));

    private void LoadSettingsFromService()
    {
        var settings = _service.Settings;
        SettingsAuthApiUrl = settings.AuthApiUrl.ToString();
        SettingsBridgeApiUrl = settings.BridgeApiUrl.ToString();
        SettingsRootPath = settings.RootPath;
        SettingsSyncIntervalMinutes = settings.SyncInterval.TotalMinutes.ToString("0.##");
        SelectedConflictStrategy =
            ConflictStrategies.FirstOrDefault(c => c.Strategy == settings.DefaulteConflictStrategy) ??
            ConflictStrategies[0];
        SettingsError = null;
    }

    [RelayCommand]
    private async Task SaveSettingsAsync()
    {
        if (!CanManageSettings)
        {
            ShowToast("Недостаточно прав", "bad");
            return;
        }

        if (!Uri.TryCreate(SettingsAuthApiUrl, UriKind.Absolute, out var authUri) ||
            authUri.Scheme is not ("http" or "https"))
        {
            SettingsError = "Укажите корректный абсолютный URL AuthServer (http/https).";
            return;
        }

        if (!Uri.TryCreate(SettingsBridgeApiUrl, UriKind.Absolute, out var bridgeUri) ||
            bridgeUri.Scheme is not ("http" or "https"))
        {
            SettingsError = "Укажите корректный абсолютный URL BridgeServer (http/https).";
            return;
        }

        if (string.IsNullOrWhiteSpace(SettingsRootPath))
        {
            SettingsError = "Укажите корневой каталог.";
            return;
        }

        if (!double.TryParse(SettingsSyncIntervalMinutes, out var minutes) || minutes <= 0)
        {
            SettingsError = "Интервал синхронизации должен быть положительным числом минут.";
            return;
        }

        SettingsError = null;
        IsSavingSettings = true;
        try
        {
            await _service.SaveSettingsAsync(new EdoSettings
            {
                AuthApiUrl = authUri,
                BridgeApiUrl = bridgeUri,
                RootPath = SettingsRootPath.Trim(),
                DefaulteConflictStrategy = SelectedConflictStrategy?.Strategy ?? EnumConflictStrategy.KeepLocal,
                SyncInterval = TimeSpan.FromMinutes(minutes),
            });

            LoadSettingsFromService();
            ShowToast("Настройки сохранены", "ok");
            RefreshAll();
        }
        catch (Exception ex)
        {
            ShowToast("Не удалось сохранить настройки: " + ex.Message, "bad");
        }
        finally
        {
            IsSavingSettings = false;
        }
    }
}
