using GamHubApp.ViewModels;

namespace GamHubApp.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AboutPage : ContentPage
{
    public AboutPage()
    {
        InitializeComponent();

        BindingContext = new AboutViewModel();
    }

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {

    }

    private async void Button_Clicked(object sender, EventArgs e)
    {
        var url = ((ImageButton)sender).CommandParameter as string;
        await Browser.OpenAsync(url, new BrowserLaunchOptions
        {
            LaunchMode = BrowserLaunchMode.External,
            TitleMode = BrowserTitleMode.Default,
        });
    }
}