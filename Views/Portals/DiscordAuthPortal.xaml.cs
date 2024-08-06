using GamHubApp.Core;
using GamHubApp.Models;
using GamHubApp.Models.Http.Responses;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace GamHubApp.Views.Portals
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DiscordAuthPortal : ContentPage
    {

        public Action<AuthResponse> CallBack { get; private set; }
        public string Url { get; set; }
        public bool? Result { get; set; } = null;
        public static readonly App CurrentApp = (App.Current as App);
        public DiscordAuthPortal(Action<AuthResponse> callBack)
        {
            InitializeComponent();

            Url = $"https://discord.com/oauth2/authorize?client_id={AppConstant.DiscordClientId}&response_type=code&redirect_uri=https%3A%2F%2F{AppConstant.ApiHost}%2Fauth%2Fdiscord&scope=email+identify+connections";
            CallBack = callBack;
            BindingContext = this;
        }
        protected override void OnAppearing()
        {
            if (!CurrentApp.IsLoading)
                CurrentApp.ShowLoadingIndicator();

            base.OnAppearing();
        }
        private async void DiscordPortal_Navigated(object sender, WebNavigatedEventArgs e)
        {

            

            // Remove loding indicator
            if (e.Url.Contains("discord.com/oauth2/authorize"))
            {
                CurrentApp.RemoveLoadingIndicator();

                return;
            }


            // Catch the navigation to the api
            if (e.Url.Contains($"//{AppConstant.ApiHost}/auth/discord"))
            {

                // Get data returned
                string value = Regex.Unescape(await DiscordPortal.EvaluateJavaScriptAsync("document.getElementsByTagName(\"pre\")[0].innerHTML"));
                var res = JsonConvert.DeserializeObject<AuthResponse>(value);

                // Indicate a result is being returned
                Result = true;
                CallBack(res);

                // Navigate back
                CurrentApp.MainPage.Navigation.RemovePage(this);

                CurrentApp.RemoveLoadingIndicator();
                return;
            }
        }

        private void DiscordPortal_Navigating(object sender, WebNavigatingEventArgs e)
        {
            // Catch the navigation to the api
            if (e.Url.Contains($"//{AppConstant.ApiHost}/auth/discord"))
            {

                if (!CurrentApp.IsLoading)
                    CurrentApp.ShowLoadingIndicator();
                DiscordPortal.IsVisible = false;
            }
        }
    }
}