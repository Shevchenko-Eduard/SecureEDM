using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;
using SecureEDM.Client.Presentation.ViewModels;

namespace SecureEDM.Client.Presentation.Views;

public partial class KeysView : UserControl
{
    public KeysView()
    {
        InitializeComponent();
    }

    private async void OnCopyPemClick(object? sender, RoutedEventArgs e)
    {
        if (DataContext is not MainViewModel vm) return;

        var clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
        if (clipboard is null)
        {
            vm.ShowToast("Буфер обмена недоступен", "bad");
            return;
        }

        try
        {
            await clipboard.SetTextAsync(vm.SelectedKeyPem);
            vm.ShowToast("PEM скопирован", "ok");
        }
        catch
        {
            vm.ShowToast("Не удалось скопировать", "bad");
        }
    }
}
