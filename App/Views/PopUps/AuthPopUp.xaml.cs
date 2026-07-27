
using CommunityToolkit.Maui.Views;
using GamHubApp.Models.Http.Responses;
using GamHubApp.Views.Portals;

namespace GamHubApp.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class AuthPopUp : Popup
{
    public Action<AuthResponse> CallBack { get; private set; }
    public AuthPopUp(Action<AuthResponse> callBack)
		{
        CurrentApp = (App)App.Current;

        InitializeComponent ();

        CallBack = callBack;
    }

    public App CurrentApp { get; private set; }
    private async void Discord_Clicked(object sender, System.EventArgs e)
    {
        // Close this popup
        await this.CloseAsync();

        CurrentApp.ShowLoadingIndicator();

        await Shell.Current.Navigation.PushAsync(new DiscordAuthPortal(CallBack));

    }
}