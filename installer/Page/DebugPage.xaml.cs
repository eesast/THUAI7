using installer.ViewModel;

namespace installer.Page;

public partial class DebugPage : ContentPage
{
    public DebugPage(DebugViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}