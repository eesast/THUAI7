using installer.ViewModel;

namespace installer.Page;

public partial class LaunchPage : ContentPage
{
    public LaunchPage(LaunchViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}