using Avalonia.Controls;

namespace SecureEDM.Client.Presentation.Views;

public partial class DocumentsView : UserControl
{
    public DocumentsView()
    {
        InitializeComponent();
    }

    public void FocusSearch()
    {
        DocumentSearchBox.Focus();
        DocumentSearchBox.SelectAll();
    }
}
