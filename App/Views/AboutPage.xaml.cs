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
}