using AresNews.Views;
using MvvmHelpers;
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

                        // CLose flyout 
                        MainShell.FlyoutIsPresented = false;

                        // Open the login pop up
                        CurrentApp.OpenPopUp(new AuthPopUp((res) =>
                        {
                            // Save user info
                            CurrentApp.SaveUserInfo(res.UserData);

                            // Save the session
                            CurrentApp.DataFetcher.SaveSession(res.Session);
                        }));
                    }


                });
            }
        }

        public App CurrentApp { get; private set; }
        public AppShell MainShell { get; }

        public AppShellViewModel(AppShell shell)
        {
            CurrentApp = (App)App.Current;
            MainShell = shell;
        }

    }
}
