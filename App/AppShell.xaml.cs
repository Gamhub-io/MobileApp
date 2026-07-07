using GamHubApp.ViewModels;
using GamHubApp.Views;
using Plugin.FirebasePushNotifications;

namespace GamHubApp;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AppShell : Shell
{
    private App _currentApp;
    private AppShellViewModel _vm;

    public AppShell(AppShellViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnNavigating(ShellNavigatingEventArgs args)
    {
        base.OnNavigating(args);
        try
        {
            if (args.Current?.Location == args?.Target?.Location)
            {
            if (this.CurrentPage is NewsPage)
            {
                // scroll the feed all the way to the top
                (this.CurrentPage as NewsPage).ScrollFeed();
            }
                return;
            }

        } 
        catch (Exception ex)
        {
#if DEBUG
            throw new Exception(ex.Message, ex);
#else
        SentrySdk.CaptureException(ex);
#endif
        }

            Task.Run (async () =>
            { 
            string target = args?.Target?.Location?.OriginalString;
            string origin = args?.Current?.Location?.OriginalString;
#if IOS
            if (origin?.Contains(nameof(GemTopUpPage)) ?? false)
                    await RefreshGems();
#endif
            if (string.IsNullOrEmpty(target) || string.IsNullOrEmpty(origin))
                return;
                if (_vm != null && target != "//MyDealsPage" && !target.Contains("ArticlePage"))
                    await _vm.UpdateDeals();

            });
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

       IFirebasePushNotification.Current.RegisterNotificationCategories(new[]
       {
            new NotificationCategory("daily_catchup", new[]
            {
                new NotificationAction("open_in_app", "Open in the app", NotificationActionType.Foreground),
                new NotificationAction("open_in_browser", "Open in the browser", NotificationActionType.Foreground),
            })
        });

        _currentApp = (App.Current as App);
        Task gemTask = _vm.UpdateGems();
        Task dealTask = _vm.UpdateDeals(); 
        Task partnersTask = _currentApp.LoadPartners();
        Task notifTask = _vm.NotificationSetup();
        await Task.WhenAll(dealTask, gemTask, partnersTask, notifTask);
        _vm.Appearing();
    }

    private void Auth_Tapped(object sender, TappedEventArgs e)
    {
        // CLose flyout 
        FlyoutIsPresented = false;

        // Open the login pop up
        _currentApp.OpenPopUp(new AuthPopUp((res) =>_vm.PostAuthProcess(res)), this);
    }

    private async void Logout_Tapped(object sender, TappedEventArgs e)
    {
        if (await _currentApp.ShowLogoutConfirmation())
        {
            // Remove the authenticated flag
            _vm.Authenticated = false;
            // Logout the user
            _currentApp.LogoutCurrentAccount();

        }
    }

    public void Resume()
    {
        _vm.UpdateDeals().GetAwaiter();
    }

    /// <summary>
    /// Refresh the gems balance of the user
    /// </summary>
    /// <returns></returns>
    public async Task RefreshGems()
    {
        await _vm.UpdateGems();

    }
}