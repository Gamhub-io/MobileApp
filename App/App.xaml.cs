using CommunityToolkit.Maui.Views;
using GamHubApp.Models;
using GamHubApp.Services;
using GamHubApp.ViewModels;
using GamHubApp.Views;
using GamHubApp.Views.PopUps;
using Newtonsoft.Json;
using SQLite;
using System.Collections.ObjectModel;

#if DEBUG
using System.Diagnostics;
using GamHubApp.Core;
#endif

namespace GamHubApp;

public partial class App : Application
{
    public bool IsLoading { get; private set; }

    public static Collection<Source> Sources { get; private set; }
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
    public App()
    {

#if DEBUG
            // Run the debug setup
            EnvironementSetup.DebugSetup();
#endif
       DataFetcher = new Fetcher();

       Sources = new Collection<Source>();

       InitializeComponent();

       // Start the db
       StartDb();

        



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
        /// Function to start the data base
        /// </summary>
        public static void StartDb()
        {
            // Just use whatever directory SpecialFolder.Personal returns
            string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            GeneralDBpath = Path.Combine(libraryPath, "ares.db3");
            PathDBBackUp = Path.Combine(libraryPath, "aresBackup.db3");

            // Verify if a data base already exist
            if (!File.Exists(GeneralDBpath))
                // Create the folder path.
                File.Create(GeneralDBpath);

            // Verify if a data base already exist
            if (!File.Exists(PathDBBackUp))
                // Create the folder path.
                File.Create(PathDBBackUp);

        // Sqlite connection
        //(SqLiteConn = new SQLiteConnection(GeneralDBpath)) ;
        //(BackUpConn = new SQLiteConnection(PathDBBackUp)) ;
        }

        protected override void OnStart()
        {
        using (var maincon = new SQLiteConnection(GeneralDBpath))
        {
            Thread.Sleep(1000);
            maincon.CreateTable<Source>();
            Thread.Sleep(1000);
            maincon.CreateTable<Article>();
            Thread.Sleep(1000);
            maincon.CreateTable<Feed>();

        };
        using (var maincon = new SQLiteConnection(PathDBBackUp))
        {
            maincon.CreateTable<Source>();
            Thread.Sleep(1000);
            maincon.CreateTable<Article>();
        };

        LoadingIndicator = new LoadingPopUp();

        // Task to get all the resource data from the API
        Task.Run(async () =>
        {
            Sources = await DataFetcher.GetSources();
            var threads = new List<Task>();

            foreach (var source in Sources)
            {
                using var mainConn = new SQLiteConnection(GeneralDBpath);
                mainConn.InsertOrReplace(source);

                mainConn.Close();
                using var backupConn = new SQLiteConnection(PathDBBackUp);
                backupConn.InsertOrReplace(source);

                backupConn.Close();
            }
        });

        // Restore session
        _ = DataFetcher.RestoreSession();

            // Register the date of the first run
            DateFirstRun = Preferences.Get(nameof(DateFirstRun), DateTime.MinValue);
            if (DateFirstRun == DateTime.MinValue)
            {
                // Set the property
                DateFirstRun = DateTime.Now;

                // Register this date as the first date
                Preferences.Set("date", DateFirstRun);

            }

        //Task.Run(async () =>
        //    Partners = await DataFetcher.GetPartners()).GetAwaiter();
        }
    protected override Window CreateWindow(IActivationState activationState)
    {
        return new Window(new AppShell());
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

            if (currentPage.ToString() == "GamHubApp.Views.ArticlePage")
            {

                ((ArticleViewModel)((ArticlePage)currentPage).BindingContext).TimeSpent.Start();
            }

            StartDb();
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
                if (page.Navigation.NavigationStack.Any(p => p?.Id == popUp?.Id))
                    return;
            MainThread.BeginInvokeOnMainThread(() =>
                {
                    page.ShowPopup(popUp);
                });
        }
#if DEBUG
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
#else
            catch 
            {
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
        return mainPage.CurrentPage;
    }

    /// <summary>
    /// Load all the partners
    /// </summary>
    /// <returns></returns>
    public async Task LoadPartners()
    {
        Partners = await DataFetcher.GetPartners();
        Deals = await DataFetcher.GetDeals();

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
            user = SaveInfo;

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
