using installer.ViewModel;

namespace installer.Page;

public partial class PlaybackPage : ContentPage
{
    public PlaybackPage(PlaybackViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}