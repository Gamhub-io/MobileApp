using GamHubApp.Core;
using GamHubApp.Models;
using GamHubApp.Models.Http.Responses;
using GamHubApp.Services;
using GamHubApp.Services.UI;
using GamHubApp.Views;
using Microsoft.Extensions.Logging;
using Plugin.FirebasePushNotifications;
using System.Diagnostics;

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

    public App CurrentApp { get; private set; }
    private Fetcher dataFetcher;
    private ILogger<AppShellViewModel> _logger;
    private IFirebasePushNotification _firebasePushNotification;
    private INotificationPermissions _firebasePushPermissions;
    private GeneralDataBase _generalDB;

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

    private bool _dealEnabled;
    public bool DealEnabled
    {
        get
        {
            return _dealEnabled;
        }
        set
        {
            _dealEnabled = value;
            OnPropertyChanged(nameof(DealEnabled));
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

    private int _newDealsCount;
    public int NewDealsCount
    {
       get
       {
           return _newDealsCount;
       }
       set
       {
           _newDealsCount = value;
           OnPropertyChanged(nameof(NewDealsCount));
       }    
    }

    public AppShellViewModel(Fetcher fetc,
    ILogger<AppShellViewModel> logger,
    IFirebasePushNotification firebasePushNotification,
    INotificationPermissions firebasePushPermission,
        GeneralDataBase generalDB)
    {
        CurrentApp = App.Current as App;
        dataFetcher = fetc;

        _logger = logger;
        
        _firebasePushNotification = firebasePushNotification;
        _firebasePushPermissions = firebasePushPermission;
        (_generalDB = generalDB).Init().GetAwaiter();

        Task.Run(async () =>
        {
            await fetc.RestoreSession();
            UserProfile = fetc.UserData;
        });

    }

    /// <summary>
    /// Update the deals stored in the database
    /// </summary>
    /// <returns></returns>
    public async Task UpdateDeals()
    {
        DealEnabled = Preferences.Get(AppConstant.DealPageEnable, true);

        // Set the deal count
        BadgeCounterService.SetCount(await dataFetcher.UpdateDeals());
    }

    const string _notificationKey = "Notification";

    /// <summary>
    /// Setup notification
    /// </summary>
    public async Task NotificationSetup()
    {
        if ((await _firebasePushPermissions.GetAuthorizationStatusAsync() is not Plugin.FirebasePushNotifications.Model.AuthorizationStatus.Granted)
            && Preferences.Get(_notificationKey, true))
#if ANDROID
            // For android we need to make sure it runs on the main thread
            MainThread.BeginInvokeOnMainThread(async () =>
#endif 
            {
                bool newStatus = await _firebasePushPermissions.RequestPermissionAsync();
                Preferences.Set(_notificationKey, newStatus);
                if (!newStatus) return;
                await Task.Delay(1000);
                await _firebasePushNotification.RegisterForPushNotificationsAsync();
#if ANDROID
            });
#else
        }
        // TODO: remove once https://github.com/thomasgalliker/Plugin.FirebasePushNotifications/issues/113 is sorted 
        await Task.Delay(TimeSpan.FromSeconds(3));
#endif

        _firebasePushNotification.TokenRefreshed += this.OnTokenRefresh;
        _firebasePushNotification.NotificationOpened += OnNotificationOpened;
        _firebasePushNotification.NotificationAction += OnNotificationAction;
#if DEBUG
        Debug.WriteLine($"Notify token: {_firebasePushNotification.Token}");
#endif
        _firebasePushNotification.SubscribeTopic("daily_catchup");


    }

    private void OnNotificationAction(object sender, FirebasePushNotificationActionEventArgs e)
    {

        if (e.Action?.Id == null)
            return;

        switch (e.Action.Id)
        {
            case "open_in_app":
                OpenArticleInApp(e.Data["articleId"].ToString());
                break;
            case "open_in_browser":
                MainThread.BeginInvokeOnMainThread(async () =>
                    await Browser.OpenAsync(e.Data["url"].ToString(), new BrowserLaunchOptions
                    {
                        LaunchMode = BrowserLaunchMode.SystemPreferred,
                        TitleMode = BrowserTitleMode.Default,
                    }));
                break;
            default:
                OpenArticleInApp(e.Data["articleId"].ToString());
                break;


        }
    }


    /// <summary>
    /// Event that gets triggered when a notification is opened
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnNotificationOpened(object sender, FirebasePushNotificationResponseEventArgs e)
    {
        if (e.Data?.Count <= 0)
            return;
        if (e.Data.ContainsKey ("articleId"))
            OpenArticleInApp(e.Data["articleId"].ToString());

    }

    /// <summary>
    /// Open an article in the app 
    /// </summary>
    /// <param name="articleId">id of the article to open</param>
    private void OpenArticleInApp(string articleId)
    {
        (App.Current as App).ShowLoadingIndicator();
        if (articleId is null) 
        {
            (App.Current as App).RemoveLoadingIndicator();
            return;
        }

        // Handle the nafication to the page on the main thread
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            Article article = await dataFetcher.GetArticle(articleId);

            if (article is null)
            {
                (App.Current as App).RemoveLoadingIndicator();
                return;
            }

            var articlePage = new ArticlePage(article);

            await App.Current.Windows[0].Page.Navigation.PushAsync(articlePage);
            (App.Current as App).RemoveLoadingIndicator();

        });
    }

    private void OnTokenRefresh(object sender, FirebasePushNotificationTokenEventArgs e)
    {
#if DEBUG
        Debug.WriteLine($"Notify token: {e.Token}");
#endif

        //this.UpdateSubscribedTopics();
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
