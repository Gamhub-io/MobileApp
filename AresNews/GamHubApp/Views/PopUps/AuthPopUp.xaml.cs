
using CommunityToolkit.Maui.Views;
using GamHub.Models.Http.Responses;
using GamHub.Views.Portals;

namespace GamHub.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AuthPopUp : Popup
    {
        public Action<AuthResponse> CallBack { get; private set; }
        public AuthPopUp(Action<AuthResponse> callBack)
		{
			InitializeComponent ();

            CallBack = callBack;
		}

        public App CurrentApp { get; private set; }

        private async void Discord_Clicked(object sender, System.EventArgs e)
        {
            CurrentApp.ShowLoadingIndicator();
            DiscordAuthPortal discordAuthPortal = new(CallBack);


            await App.Current.MainPage.Navigation.PushAsync(discordAuthPortal);

            // Close this popup
            //CurrentApp.ClosePopUp(this);
            this.Close();
        }
    }
}