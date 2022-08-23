using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AresNews.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public AboutViewModel()
        {
        }

        public Command ViewGithub
        {
            get
            {
                return new Command(async () => await Browser.OpenAsync("https://github.com/bricefriha/AresGaming", new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Default,
                }));
            }
        }

        public Command ViewTwitter
        {
            get
            {
                return new Command(async (username) => await Browser.OpenAsync($"https://twitter.com/{username}", new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Default,
                }));
            }
        }
    }
}
