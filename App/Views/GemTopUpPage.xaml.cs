namespace GamHubApp.Views;

public partial class GemTopUpPage : ContentPage
{
	public GemTopUpPage()
	{
		InitializeComponent();
	}

    private async void Button_Clicked(object sender, EventArgs e)
    {
        await DisplayAlert("Confirm Your In-App Purchase", "Do you want to buy 20 Gems for $2.99 \n [Environment: SandBox]", "Cancel", "Buy");
    }
}