using GamHubApp.Models;
using GamHubApp.Models.Http.Responses;
using GamHubApp.Services;

namespace GamHubApp.ViewModels;

public class AppShellViewModel : BaseViewModel
{

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


    public App CurrentApp { get; private set; }

    private Fetcher dataFetcher;

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


    public AppShellViewModel(Fetcher fetc)
    {
        CurrentApp = App.Current as App;
        dataFetcher = fetc;


        Task.Run(async () =>
        {
            await fetc.RestoreSession();
            UserProfile = fetc.UserData;

        });
    }

    public void PostAuthProcess(AuthResponse res) 
    {
        // Save user info
        dataFetcher.SaveUserInfo(res.UserData);

        // Save the session
        dataFetcher.SaveSession(res.Session);

        // Set user data
        UserProfile = res.UserData;
    }

}
