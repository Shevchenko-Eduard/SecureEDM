using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using SecureEDM.Client.Presentation.ViewModels;

namespace SecureEDM.Client.Presentation.Views;

public partial class SigningView : UserControl
{
    public SigningView()
    {
        InitializeComponent();
    }

    private async void OnDownloadPackageClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;

        var json = vm.BuildSignaturePackageJson();
        if (string.IsNullOrEmpty(json))
        {
            vm.ShowToast("Сначала подпишите документ", "bad");
            return;
        }

        var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
        if (storage is null) return;

        var file = await storage.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "Сохранить пакет подписи",
            SuggestedFileName = vm.SignaturePackageFileName,
            DefaultExtension = "json",
            FileTypeChoices =
            [
                new FilePickerFileType("Пакет подписи")
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
            await writer.WriteAsync(json);
            vm.ShowToast("Пакет подписи сохранён", "ok");
        }
        catch (Exception ex)
        {
            vm.ShowToast("Не удалось сохранить пакет: " + ex.Message, "bad");
        }
    }
}
