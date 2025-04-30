using GamHubApp.Services.UI;
using GamHubApp.ViewModels;

namespace GamHubApp.Views;

public partial class DealsPage : ContentPage
{
	public DealsPage(DealsViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
    protected override void OnAppearing()
	{
        BadgeCounterService.SetCount(0);
    }
}