namespace installer;

public partial class CompetitionPage : ContentPage
{
	public CompetitionPage()
	{
		InitializeComponent();
	}

    private async void JumpBtn_Clicked(object sender, EventArgs e)
    {
		await Navigation.PopToRootAsync();
    }
}