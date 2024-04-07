using AresNews.Views;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AresNews.ViewModels
{
    public class AppShellViewModel : BaseViewModel
    {
        private int logoClickCount;

        public Command MailTo
        {
            get
            {
                return new Command<string>(async (address) => await Email.ComposeAsync(subject: "", body: "", to: new string[] { address }));
            }
        }
        public Command OpenAuth
        {
            get
            {
                return new Command( () =>
                {
                    logoClickCount++;

                    if (logoClickCount == 3)
                    {
                        // Reset the logo click count
                        logoClickCount = 0;

                        // Open the login pop up
                        CurrenApp.OpenPopUp(new AuthPopUp());
                    }


                });
            }
        }

        public App CurrenApp { get; private set; }

        public AppShellViewModel()
        {
            CurrenApp = (App)App.Current;
        }

    }
}
