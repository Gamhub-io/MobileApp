using GamHubApp.Core;
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
        Preferences.Set(PreferencesKeys.NewDealCount, 0);
        (App.Current as App).ShowLoadingIndicator();

        // Note: this is a workaround for a MAUI 10 bug that prevents the tabbar colours to set proper
        Dispatcher.Dispatch(() =>
        {
            Shell.SetTabBarBackgroundColor(this,
                (Color)Application.Current.Resources["LightDark"]);

            Shell.SetTabBarForegroundColor(this,
                (Color)Application.Current.Resources["Primary"]);

            Shell.SetTabBarTitleColor(this,
                (Color)Application.Current.Resources["Primary"]);

            Shell.SetTabBarUnselectedColor(this,
                (Color)Application.Current.Resources["UnselectedTabFont"]);
        });

        Resume();
    }
    public void Resume()
    {
        _vm.UpdateDeals().GetAwaiter();
    }
}