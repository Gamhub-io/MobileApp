using CommunityToolkit.Maui.ApplicationModel;
using CommunityToolkit.Maui.Views;
using GamHubApp.Core;
using GamHubApp.Models;
using GamHubApp.Services;
using GamHubApp.ViewModels;
using GamHubApp.Views;
using GamHubApp.Views.PopUps;
using Newtonsoft.Json;
using Plugin.FirebasePushNotifications;
using System.Collections.ObjectModel;


#if DEBUG
using System.Diagnostics;
#endif

namespace GamHubApp;

public partial class App : Application
{
    private GeneralDataBase _generalDb;
    private BackUpDataBase _backupDb;
    private Window _window;

    public bool IsLoading { get; private set; }
    private AppShell Shell { get; set; }
    public Popup LoadingIndicator { get; private set; }
    public Fetcher DataFetcher { get; set; }
    public static string ProdHost { get; } = "api.gamhub.io";
    public static string LocalHost { get; } = "gamhubdev.ddns.net";
    public User SaveInfo { get; private set; }
    public Collection<Partner> Partners { get; private set; }
    public Collection<Deal> Deals { get; private set; }
    /// <summary>
    /// Date first registered to determin when is the best time to ask for user review
    /// </summary>
    public DateTime DateFirstRun { get; set; }
    public static string GeneralDBpath { get; private set; }
    public static string PathDBBackUp { get; private set; }

    public enum PageType
    {
        about,
        article,
        bookmark,
        news,
        source
    }
    public App(Fetcher fetc, AppShell shell, GeneralDataBase generalDataBase, BackUpDataBase backUpDataBase)
    {
        _generalDb = generalDataBase;
        _backupDb = backUpDataBase;

       DataFetcher = fetc;
       Shell = shell;

       InitializeComponent();
    }

    /// <summary>
    /// Show the popup loading indicator
    /// </summary>
    public void ShowLoadingIndicator(Page rootPage = null)
    {

       if (IsLoading)
           return;
       IsLoading = true;

       OpenPopUp (this.LoadingIndicator = new LoadingPopUp(), rootPage);

    }

    /// <summary>
    /// Remove the popup loading indicator
    /// </summary>
    public void RemoveLoadingIndicator()
    {
       if (!IsLoading)
           return;


       IsLoading = false;
       
       // Close the popup
        MainThread.BeginInvokeOnMainThread(() =>
        {
           this.LoadingIndicator.Close();
        });
    }

    /// <summary>
    /// Process ran if the app was previously used in offline mode
    /// </summary>
    private async Task RecoverFromOffline()
    {
        bool connectivity = Connectivity.NetworkAccess == NetworkAccess.Internet;
        if (Preferences.Get(AppConstant.OfflineLastRun, false) &&
            connectivity)
        {
            var feeds = new ObservableCollection<Feed>(await _generalDb.GetFeeds());
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < feeds.Count; i++)
                tasks.Add(DataFetcher.UpdateFeed(feeds[i]));

            await Task.WhenAll(tasks);
        }
        Preferences.Set(AppConstant.OfflineLastRun, !connectivity);
    }

    protected override void OnStart()
    {
        Task.Run(RecoverFromOffline);
        // Register the date of the first run
        DateFirstRun = Preferences.Get(nameof(DateFirstRun), DateTime.MinValue);
        if (DateFirstRun == DateTime.MinValue)
        {
            // Set the property
            DateFirstRun = DateTime.Now;

            // Register this date as the first date
            Preferences.Set("date", DateFirstRun);

        }

        IFirebasePushNotification.Current.RegisterNotificationCategories(new[]
        {
            new NotificationCategory("daily_catchup", new[]
            {
                new NotificationAction("open_in_app", "Open in the app", NotificationActionType.Foreground),
                new NotificationAction("open_in_browser", "Open in the browser", NotificationActionType.Foreground),
            })
        });

        // Reset notificaiton badges
        Badge.Default.SetCount(0);
    }
    protected override Window CreateWindow(IActivationState activationState)
    {
        LoadingIndicator = new LoadingPopUp();

        _generalDb.Init()?.GetAwaiter();
        _backupDb.Init()?.GetAwaiter();
        DataFetcher.LoadBookmarks().GetAwaiter();
        DataFetcher.UpdateBackupSources().GetAwaiter();
        try
        {

            return _window = new Window(Shell);
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#else
            SentrySdk.CaptureException(ex);
#endif
            return _window;

        }
    }
    protected override void OnSleep()
     {
         base.OnSleep();

         AppShell mainPage = ((AppShell)Current.Windows[0].Page);
         Page currentPage = mainPage.CurrentPage;

         if (currentPage.ToString() == "GamHubApp.Views.ArticlePage")
         {

             ((ArticleViewModel)((ArticlePage)currentPage).BindingContext).TimeSpent.Stop();
         }
     }
     

     protected override void OnResume()
     {
         AppShell mainPage = ((AppShell)Current.Windows[0].Page);
         Page currentPage = mainPage.CurrentPage;
         mainPage.Resume();

         if (currentPage.ToString() == "GamHubApp.Views.ArticlePage")
         {

             ((ArticleViewModel)((ArticlePage)currentPage).BindingContext).TimeSpent.Start();
         }

         if (currentPage.ToString() == "GamHubApp.Views.NewsPage")
         {

             ((NewsPage)currentPage).OnResume();
         }

        if (currentPage.ToString() == "GamHubApp.Views.DealsPage")
        {

            ((DealsPage)currentPage).Resume();
        }

        // Reset notificaiton badges
        Badge.Default.SetCount(0);
    }
     /// <summary>
     /// Open any popup
     /// </summary>
     /// <param name="popUp">pop up to open</param>
     /// <param name="page">parent page</param>
     public void OpenPopUp(Popup popUp, Page page = null)
     {
         try
         {

             if (popUp == null)
                 return;

             if (page == null)
                 page = GetCurrentPage();
             if (page.Navigation.NavigationStack.Any(p => p?.Id == popUp!.Id))
                 return;
         MainThread.BeginInvokeOnMainThread(() => page.ShowPopup(popUp));
        }
#if DEBUG
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
#else
        catch (Exception ex) 
        {
            SentrySdk.CaptureException(ex);
        }
#endif
    }
    /// <summary>
    /// Get the current page from the shell
    /// </summary>
    /// <returns></returns>
    private Page GetCurrentPage ()
    {
        AppShell mainPage = ((AppShell)Current.Windows[0].Page);
        return mainPage;
    }

    /// <summary>
    /// Load all the partners
    /// </summary>
    /// <returns></returns>
    public async Task LoadPartners()
    {
        Partners = await DataFetcher.GetPartners();

    }

    /// <summary>
    /// Save the info relevant to the user
    /// </summary>
    public void SaveUserInfo(User user)
    {
        SaveInfo = user;

        // Save preferences
        Preferences.Set(nameof(SaveInfo), JsonConvert.SerializeObject(SaveInfo));
    }
    /// <summary>
    /// Save the info relevant to the user
    /// </summary>
    public void DeleteUserInfo()
    {
        // Save preferences
        Preferences.Remove(nameof(SaveInfo));
    }
    /// <summary>
    /// Recover the info relevant to the user
    /// </summary>
    /// <returns>true: data found | false: data not found</returns>
    public bool RecoverUserInfo()
    {

        // Get preferences
        string userDataStr = Preferences.Get(nameof(SaveInfo), string.Empty);

        if (string.IsNullOrEmpty(userDataStr))
            return false;

        // Set Userdata object
        return (SaveInfo = DataFetcher.UserData = JsonConvert.DeserializeObject<User>(userDataStr)) != null;
    }

    /// <summary>
    /// Log out the current active user
    /// </summary>
    public void LogoutCurrentAccount()
    {
        // Delete user data
        DeleteUserInfo();

        // Close the current session
        DataFetcher.KillSession();
    }

    /// <summary>
    /// Show a pop up to confirm wether or not the user wants to logout
    /// </summary>
    /// <param name="user">user</param>
    /// <returns>true: confirm | false: cancel</returns>
    public async Task<bool> ShowLogoutConfirmation(User user = null)
    {
        if (user == null)
            user = DataFetcher.UserData;

        LogoutConfirmationPopUp popUp = new(user);
        OpenPopUp(popUp);

        // Wait for a response
        while (popUp.ResponseResult == null)
            await Task.Delay(10);

        // in any case close the pop up after receiving a response
        popUp.Close();

        return popUp.ResponseResult ?? false;
    }

}
