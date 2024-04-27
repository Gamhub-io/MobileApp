
using AresNews.Models.Http.Responses;
using AresNews.Views.Portals;
using Rg.Plugins.Popup.Pages;
using System;
using Xamarin.Forms.Xaml;

namespace AresNews.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AuthPopUp : PopupPage
    {
        public Action<AuthResponse> CallBack { get; private set; }
        public AuthPopUp(Action<AuthResponse> callBack)
		{
			InitializeComponent ();

            CallBack = callBack;
		}

        public App CurrentApp { get; private set; }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            
            CurrentApp = App.Current as App;
            // prevent the page from collapsing when the keyboard appears
            HasSystemPadding = false;
        }

        private void Discord_Clicked(object sender, System.EventArgs e)
        {
            // Close this popup
            CurrentApp.ClosePopUp(this);
            DiscordAuthPortal discordAuthPortal = new (CallBack);
               

            _ = App.Current.MainPage.Navigation.PushAsync(discordAuthPortal);
        }
    }
}