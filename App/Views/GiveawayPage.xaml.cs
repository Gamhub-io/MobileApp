using GamHubApp.ViewModels;

namespace GamHubApp.Views;

public partial class GiveawayPage : ContentPage
{
	public GiveawayPage(GiveawayViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;

    }
}