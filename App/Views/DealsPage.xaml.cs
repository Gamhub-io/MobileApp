using GamHubApp.Services.UI;
using GamHubApp.ViewModels;

namespace GamHubApp.Views;

public partial class DealsPage : ContentPage
{
    private DealsViewModel _vm;

    public DealsPage(DealsViewModel vm)
	{
		InitializeComponent();
		BindingContext = _vm = vm;
	}
    protected override void OnAppearing()
	{
        BadgeCounterService.SetCount(0);
		(App.Current as App).ShowLoadingIndicator();
        _vm.UpdateDeals().GetAwaiter();
    }
}