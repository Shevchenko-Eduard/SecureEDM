using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using SecureEDM.Client.Presentation.ViewModels;

namespace SecureEDM.Client.Presentation.Views;

public partial class LogView : UserControl
{
    public LogView()
    {
        InitializeComponent();
    }

    private async void OnExportRegistryClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;
        if (!vm.CanExport)
        {
            vm.ShowToast("Недостаточно прав", "bad");
            return;
        }

        var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
        if (storage is null) return;

        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Экспорт реестра",
            SuggestedFileName = $"edo-registry-{DateTime.Now:yyyy-MM-dd}.json",
            DefaultExtension = "json",
            FileTypeChoices =
            [
                new FilePickerFileType("Реестр ЭДО")
                {
                    Patterns = ["*.json"],
                    MimeTypes = ["application/json"],
                },
            ],
        });

        if (file is null) return;

        try
        {
            await using var stream = await file.OpenWriteAsync();
            await using var writer = new StreamWriter(stream);
            await writer.WriteAsync(vm.ExportRegistryJson());
            vm.ShowToast("Реестр экспортирован", "ok");
        }
        catch (Exception ex)
        {
            vm.ShowToast("Не удалось экспортировать реестр: " + ex.Message, "bad");
        }
    }
}
