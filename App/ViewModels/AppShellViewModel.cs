using GamHubApp.Models;
using GamHubApp.Views;

namespace GamHubApp.ViewModels
{
    public class AppShellViewModel : BaseViewModel
    {
        private int logoClickCount;

        public Command MailTo
        {
            get
            {
                return new Command<string>((address) => _ = Email.ComposeAsync(subject: "", body: "", to: new string[] { address }));
            }
        }

        public Command Logout
        {
            get
            {
                return new Command(async () => 
                {

                    if (await CurrentApp.ShowLogoutConfirmation())
                    {
                        // Remove the authenticated flag
                        Authenticated = false;
                        // Logout the user
                        CurrentApp.LogoutCurrentAccount();

                    }
                });
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

                            // Set user data
                            UserProfile = res.UserData;
                        }), MainShell);
                    }
                });
            }
        }

        public App CurrentApp { get; private set; }
        public AppShell MainShell { get; }
        private bool _authenticated;

        public bool Authenticated
        {
            get { return _authenticated; }
            set 
            {
                _authenticated = value;
                OnPropertyChanged(nameof(Authenticated));
            }
        }

        private User _userProfile;
        public User UserProfile
        {
            get { return _userProfile; }
            set 
            {
                _userProfile = value;

                // Enable the profile view
                Authenticated = value != null;

                OnPropertyChanged(nameof(UserProfile));
            }
        }


        public AppShellViewModel(AppShell shell)
        {
            CurrentApp = (App)App.Current;
            MainShell = shell;

            UserProfile = CurrentApp.DataFetcher.UserData;
        }

    }
}
