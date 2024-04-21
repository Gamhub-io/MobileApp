using AresNews.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AresNews.Views.Portals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiscordAuthPortal : ContentPage
    {
        public string Url { get; set; }
        public DiscordAuthPortal()
        {
            InitializeComponent();

            Url = $"https://discord.com/oauth2/authorize?client_id={AppConstant.DiscordClientId}&response_type=code&redirect_uri=https%3A%2F%2F{AppConstant.ApiHost}%2Fauth%2Fdiscord&scope=email+identify+connections";
            BindingContext = this;
        }

        private void DiscordPortal_Navigated(object sender, WebNavigatedEventArgs e)
        {
            // Catch the navigation to the api
            if (e.Url.Contains($"//{AppConstant.ApiHost}/auth/discord"))
            {
                // Navigate back
                (App.Current as App).MainPage.Navigation.RemovePage(this);
            }
        }

        private void DiscordPortal_Navigating(object sender, WebNavigatingEventArgs e)
        {
            // Catch the navigation to the api
            if (e.Url.Contains($"//{AppConstant.ApiHost}/auth/discord"))
            {
                DiscordPortal.IsVisible = false;
            }
        }
    }
}