using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.VisualTree;
using SecureEDM.Client.Presentation.ViewModels;

namespace SecureEDM.Client.Presentation.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (DataContext is not MainViewModel vm) return;

        var focused = TopLevel.GetTopLevel(this)?.FocusManager?.GetFocusedElement();
        if (focused is TextBox or ComboBox or NumericUpDown) return;

        if (e.Key is >= Key.D1 and <= Key.D9)
        {
            var tab = e.Key switch
            {
                Key.D1 => "docs",
                Key.D2 => "keys",
                Key.D3 => "sign",
                Key.D4 => "verify",
                Key.D5 => "hashes",
                Key.D6 => "files",
                Key.D7 => "log",
                Key.D8 => "settings",
                _ => "about",
            };
            vm.SelectTabCommand.Execute(tab);
            e.Handled = true;
            return;
        }

        if (e.Key == Key.OemQuestion || e.Key == Key.Divide)
        {
            vm.SelectTabCommand.Execute("docs");
            this.FindDescendantOfType<DocumentsView>()?.FocusSearch();
            e.Handled = true;
            return;
        }

        if (e.Key == Key.Escape && vm.IsConfirmVisible)
        {
            vm.CancelConfirmCommand.Execute(null);
            e.Handled = true;
        }
    }
}
