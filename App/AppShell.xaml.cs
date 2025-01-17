using GamHubApp.ViewModels;
using GamHubApp.Views;

namespace GamHubApp;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();
        BindingContext = new AppShellViewModel(this);
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
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();

        await (App.Current as App).LoadPartners();
    }
}