using GamHubApp.ViewModels;
using GamHubApp.Views;

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
        if (args.Current?.Location == args.Target.Location)
        {
            if (this.CurrentPage is NewsPage)
            {
                // scroll the feed all the way to the top
                (this.CurrentPage as NewsPage).ScrollFeed();
            }
        }
        else
            Task.Run (async () =>
            { 
                string target = args.Target.Location.OriginalString;
                if (_vm != null && target != "//MyDealsPage" && !target.Contains("ArticlePage"))
                    await _vm.UpdateDeals();
            });
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        _currentApp = (App.Current as App);
        Task partnersTask = _currentApp.LoadPartners();
        Task notifTask = _vm.NotificationSetup();
        await Task.WhenAll(partnersTask, notifTask);
        await _vm.UpdateDeals(); 
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
}