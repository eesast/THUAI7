using installer.ViewModel;

namespace installer.Page;

public partial class PlaybackPage : ContentPage
{
    public PlaybackPage(LaunchViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}