using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SecureEDM.Client.Application.Services;
using SecureEDM.Client.Domain.Enums;

namespace SecureEDM.Client.Presentation.ViewModels;

public partial class MainViewModel
{
    public ObservableCollection<TrackingFileRow> TrackingFiles { get; } = [];

    [ObservableProperty]
    private string? _newTrackingPath;

    [ObservableProperty]
    private string? _newTrackingExtension;

    [ObservableProperty]
    private string? _trackingPathError;

    [ObservableProperty]
    private bool _hasTrackingFiles;

    [ObservableProperty]
    private string _emptyTrackingFilesText = "Отслеживаемых файлов пока нет.";

    [ObservableProperty]
    private bool _showDeletedTrackingFiles;

    public bool HasTrackingPathError => !string.IsNullOrEmpty(TrackingPathError);

    partial void OnTrackingPathErrorChanged(string? value) =>
        OnPropertyChanged(nameof(HasTrackingPathError));

    partial void OnShowDeletedTrackingFilesChanged(bool value) => RefreshFiles();

    [RelayCommand]
    private async Task AddTrackingFileAsync()
    {
        if (!CanManageFiles)
        {
            ShowToast("Недостаточно прав", "bad");
            return;
        }

        TrackingPathError = string.IsNullOrWhiteSpace(NewTrackingPath)
            ? "Укажите путь к файлу"
            : null;
        if (TrackingPathError is not null) return;

        try
        {
            await _service.AddTrackingFileAsync(NewTrackingPath!, NewTrackingExtension ?? string.Empty);
            NewTrackingPath = string.Empty;
            NewTrackingExtension = string.Empty;
            ShowToast("Файл добавлен в отслеживание", "ok");
            RefreshAll();
        }
        catch (Exception ex)
        {
            ShowToast("Не удалось добавить файл: " + ex.Message, "bad");
        }
    }

    [RelayCommand]
    private async Task CheckTrackingFileAsync(string? id)
    {
        if (!CanManageFiles) return;
        try
        {
            await _service.CheckTrackingFileAsync(id ?? string.Empty);
            RefreshAll();
        }
        catch (Exception ex)
        {
            ShowToast("Ошибка проверки файла: " + ex.Message, "bad");
        }
    }

    [RelayCommand]
    private async Task SyncTrackingFileAsync(string? id)
    {
        if (!CanManageFiles) return;
        try
        {
            await _service.SyncTrackingFileAsync(id ?? string.Empty);
            ShowToast("Файл синхронизирован", "ok");
            RefreshAll();
        }
        catch (Exception ex)
        {
            ShowToast("Ошибка синхронизации: " + ex.Message, "bad");
        }
    }

    [RelayCommand]
    private async Task PauseTrackingFileAsync(string? id)
    {
        if (!CanManageFiles) return;
        try
        {
            await _service.PauseTrackingFileAsync(id ?? string.Empty);
            ShowToast("Отслеживание приостановлено", "warn");
            RefreshAll();
        }
        catch (Exception ex)
        {
            ShowToast("Не удалось приостановить файл: " + ex.Message, "bad");
        }
    }

    [RelayCommand]
    private void HardDeleteTrackingFile(string? id)
    {
        if (!CanManageFiles) return;

        var file = _service.FindTrackingFile(id);
        if (file is null) return;

        ShowConfirm(
            "Удалить запись навсегда?",
            $"Запись «{file.Path}» будет полностью удалена из реестра.",
            async () =>
            {
                await _service.DeleteTrackingFileAsync(file.Id);
                ShowToast("Запись удалена", "warn");
                RefreshAll();
            });
    }

    [RelayCommand]
    private void SoftDeleteTrackingFile(string? id)
    {
        if (!CanManageFiles) return;

        var file = _service.FindTrackingFile(id);
        if (file is null || file.DeletedAt is not null) return;

        ShowConfirm(
            "Удалить отслеживание?",
            $"Файл «{file.Path}» будет помечен как удалённый. Данные журнала сохранятся.",
            async () =>
            {
                await _service.SoftDeleteTrackingFileAsync(file.Id);
                ShowToast("Отслеживание удалено", "warn");
                RefreshAll();
            });
    }

    private void RefreshFiles()
    {
        TrackingFiles.Clear();
        var files = _service.TrackingFiles
            .Where(f => ShowDeletedTrackingFiles || f.DeletedAt is null)
            .OrderByDescending(f => f.CreatedAt);

        foreach (var file in files)
        {
            var kind = file.DeletedAt is not null
                ? "none"
                : file.StatusTracking switch
                {
                    EnumStatusTracking.Active => "ok",
                    EnumStatusTracking.HashesDontMatch => "warn",
                    EnumStatusTracking.Missing => "bad",
                    EnumStatusTracking.Error => "bad",
                    _ => "none",
                };

            var text = file.DeletedAt is not null
                ? "удалён"
                : EdoService.StatusTrackingLabel(file.StatusTracking);

            TrackingFiles.Add(new TrackingFileRow(
                file.Id,
                file.Path,
                string.IsNullOrWhiteSpace(file.FixedExtension) ? "—" : file.FixedExtension,
                kind,
                text,
                file.DeletedAt is not null
                    ? $"Удалён {FormatTime(file.DeletedAt.Value)}"
                    : file.StatusTracking == EnumStatusTracking.Missing
                        ? $"Путь: {_service.GetFullTrackingPath(file)}"
                        : text,
                FormatShortTime(file.CreatedAt),
                file.LastSync is { } lastSync ? FormatShortTime(lastSync) : "—",
                file.DeletedAt is not null,
                false));
        }

        HasTrackingFiles = TrackingFiles.Count > 0;
        EmptyTrackingFilesText = ShowDeletedTrackingFiles
            ? "Записей пока нет."
            : "Отслеживаемых файлов пока нет.";
    }
}
