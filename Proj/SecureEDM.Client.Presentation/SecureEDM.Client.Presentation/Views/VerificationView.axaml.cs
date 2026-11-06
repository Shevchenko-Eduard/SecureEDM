using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Platform.Storage;
using SecureEDM.Client.Presentation.ViewModels;

namespace SecureEDM.Client.Presentation.Views;

public partial class VerificationView : UserControl
{
    public VerificationView()
    {
        InitializeComponent();

        DropZone.AddHandler(DragDrop.DragOverEvent, OnDropDragOver);
        DropZone.AddHandler(DragDrop.DragLeaveEvent, OnDropDragLeave);
        DropZone.AddHandler(DragDrop.DropEvent, OnDropDrop);
        DropZone.PointerPressed += OnDropPointerPressed;
    }

    private void OnDropDragOver(object? sender, DragEventArgs e)
    {
        var files = e.DataTransfer.TryGetFiles();
        var hasFiles = files is { Length: > 0 };
        e.DragEffects = hasFiles ? DragDropEffects.Copy : DragDropEffects.None;
        SetHot(sender, hasFiles);
        e.Handled = true;
    }

    private void OnDropDragLeave(object? sender, DragEventArgs e)
    {
        SetHot(sender, false);
        e.Handled = true;
    }

    private async void OnDropDrop(object? sender, DragEventArgs e)
    {
        SetHot(sender, false);
        e.Handled = true;

        if (DataContext is not MainViewModel vm) return;

        var files = e.DataTransfer.TryGetFiles();
        var path = files?.FirstOrDefault()?.TryGetLocalPath();
        if (!string.IsNullOrEmpty(path))
            await vm.HandlePackageFileAsync(path);
    }

    private async void OnDropPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;

        var storage = TopLevel.GetTopLevel(this)?.StorageProvider;
        if (storage is null) return;

        var files = await storage.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Выберите пакет подписи",
            AllowMultiple = false,
            FileTypeFilter =
            [
                new FilePickerFileType("Пакет подписи")
                {
                    Patterns = ["*.json"],
                    MimeTypes = ["application/json"],
                },
            ],
        });

        var path = files.FirstOrDefault()?.TryGetLocalPath();
        if (!string.IsNullOrEmpty(path))
            await vm.HandlePackageFileAsync(path);
    }

    private static void SetHot(object? sender, bool hot)
    {
        if (sender is Border border)
            border.Classes.Set("hot", hot);
    }
}
