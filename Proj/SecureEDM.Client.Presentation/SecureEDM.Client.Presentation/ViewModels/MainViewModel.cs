using CommunityToolkit.Mvvm.ComponentModel;

namespace SecureEDM.Client.Presentation.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial string Greeting { get; set; } = "Welcome to Avalonia!";
}
