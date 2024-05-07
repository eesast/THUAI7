using CommunityToolkit.Maui.Storage;
using installer.ViewModel;

namespace installer.Page;

public partial class InstallPage : ContentPage
{
    public InstallPage(InstallViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}