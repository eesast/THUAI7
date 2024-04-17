using installer.ViewModel;

namespace installer.Page;

public partial class DebugPage : ContentPage
{
	public DebugPage(LaunchViewModel viewModel)
	{
		InitializeComponent();
		BindingContext = viewModel;
	}
}