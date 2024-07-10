using GamHub.Models;
using GamHub.Services;
using GamHub.ViewModels;
using GamHub.Views;
using GamHub.Core;
using GamHub.Views.PopUps;
using CustardApi.Objects;
using System.Collections.ObjectModel;
using SQLite;

namespace GamHub
{
    public partial class App : Application
    {
        public bool IsLoading { get; private set; }

        public static Collection<Source> Sources { get; private set; }
        // Property SqlLite Connection
        public static SQLiteConnection SqLiteConn { get; set; }
        public static SQLiteConnection BackUpConn { get; set; }

        public PopupPage LoadingIndicator { get; private set; }
        public static Service WService { get; set; }
        public Fetcher DataFetcher { get; set; }
        public static string ProdHost { get; } = "api.gamhub.io";
        public static string LocalHost { get; } = "gamhubdev.ddns.net";
        public User SaveInfo { get; private set; }
        /// <summary>
        /// Date first registered to determin when is the best time to ask for user review
        /// </summary>
        public DateTime DateFirstRun { get; set; }
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
#if __LOCAL__
            // Set webservice
            WService = new Service(host: LocalHost,
                                    port: 255,
                                   sslCertificate: false);
#else
            // Set webservice
            WService = new Service(host: ProdHost,
                                   sslCertificate: true);
#endif

#if DEBUG
            // Run the debug setup
            EnvironementSetup.DebugSetup();
#endif
            DataFetcher = new Fetcher();

            //Sharpnado.Tabs.Initializer.Initialize(false, false);
            //Sharpnado.Shades.Initializer.Initialize(loggerEnable: false);
            Sources = new Collection<Source>();

            InitializeComponent();
            //Sharpnado.CollectionView.Initializer.Initialize(true, false);

            // Start the db
            StartDb();

            SqLiteConn.CreateTable<Source>();
            SqLiteConn.CreateTable<Article>();
            SqLiteConn.CreateTable<Feed>();
            BackUpConn.CreateTable<Source>();
            BackUpConn.CreateTable<Article>();

            LoadingIndicator = new LoadingPopUp();

            Task.Run(async () =>
            {
                Sources = await WService.Get<Collection<Source>>(controller: "sources", action: "getAll", unSuccessCallback: async (e) =>
                {
#if DEBUG
                    throw new Exception (await e.Content.ReadAsStringAsync());
#endif
                });


                foreach (var source in Sources)
                {
                    SqLiteConn.InsertOrReplace(source);
                    BackUpConn.InsertOrReplace(source);
                }
            });

            // Close the db
            //CloseDb();

            
        }
        /// <summary>
        /// Show the popup loading indicator
        /// </summary>
        public void ShowLoadingIndicator()
        {

            try
            {

                if (IsLoading)
                    return;
                IsLoading = true;

                OpenPopUp (this.LoadingIndicator);
            }
            catch (RGPageInvalidException)
            {
            }

        }

        /// <summary>
        /// Remove the popup loading indicator
        /// </summary>
        public void RemoveLoadingIndicator()
        {
            try 
            {
                if (!IsLoading)
                    return;


                IsLoading = false;
                
                // Close the popup
                ClosePopUp (this.LoadingIndicator);
            }
            catch (RGPageInvalidException)
            {
            }
        }
        /// <summary>
        ///  Function to close the database 
        /// </summary>
        public static void CloseDb()
        {
            SqLiteConn.Dispose();
            //SqLiteConn.Close();
        }
        /// <summary>
        /// Function to start the data base
        /// </summary>
        public static void StartDb()
        {
            const SQLite.SQLiteOpenFlags Flags =
                    // open the database in read/write mode
                    SQLite.SQLiteOpenFlags.ReadWrite |
                    // create the database if it doesn't exist
                    SQLite.SQLiteOpenFlags.Create |
                    // enable multi-threaded database access
                    SQLite.SQLiteOpenFlags.SharedCache;
            // Just use whatever directory SpecialFolder.Personal returns
            string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            var path = Path.Combine(libraryPath, "ares.db3");
            var pathBackUp = Path.Combine(libraryPath, "aresBackup.db3");

            // Verify if a data base already exist
            if (!File.Exists(path))
                // Create the folder path.
                File.Create(path);

            // Verify if a data base already exist
            if (!File.Exists(pathBackUp))
                // Create the folder path.
                File.Create(pathBackUp);

            

            
            // Sqlite connection
            SqLiteConn = new SQLiteConnection(path);
            BackUpConn = new SQLiteConnection(pathBackUp);

            

        }

        protected override void OnStart()
        {
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

            MainPage = new AppShell();
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            AppShell mainPage = ((AppShell)MainPage);
            Page currentPage = mainPage.CurrentPage;

            if (currentPage.ToString() == "GamHub.Views.ArticlePage")
            {

                ((ArticleViewModel)((ArticlePage)currentPage).BindingContext).TimeSpent.Stop();
            }
            //SqLiteConn.Dispose();
        }
        

        protected override void OnResume()
        {
            AppShell mainPage = ((AppShell)MainPage);
            Page currentPage = mainPage.CurrentPage;

            if (currentPage.ToString() == "GamHub.Views.ArticlePage")
            {

                ((ArticleViewModel)((ArticlePage)currentPage).BindingContext).TimeSpent.Start();
            }
            //else if (currentPage.ToString() == "GamHub.Views.NewPage")
            //{
            //    ((NewsViewModel)((ArticlePage)currentPage).BindingContext).FetchArticles();
            //}

            StartDb();
        }
        /// <summary>
        /// Open any popup
        /// </summary>
        /// <param name="popUp">pop up to open</param>
        /// <param name="page">parent page</param>
        public void OpenPopUp(PopupPage popUp, Page page = null)
        {
            try
            {

                if (popUp == null)
                    return;

                if (page == null)
                    page = GetCurrentPage();

                _ = page.Navigation.PushPopupAsync(popUp);
            }
            catch (Exception ex)
            {
#if DEBUG
                throw ex;
#endif
            }
        }
        /// <summary>
        /// Close any popup
        /// </summary>
        /// <param name="popUp">pop up to open</param>
        /// <param name="page">parent page</param>
        public void ClosePopUp(PopupPage popUp, Page page = null)
        {
            try
            {

                if (popUp == null)
                    return;

                if (page == null)
                    page = GetCurrentPage();

                _ = page.Navigation.RemovePopupPageAsync(popUp);
            }
            catch (Exception ex)
            {
#if DEBUG
                throw ex;
#endif
            }
        }
        /// <summary>
        /// Get the current page from the shell
        /// </summary>
        /// <returns></returns>
        private Page GetCurrentPage ()
        {
            AppShell mainPage = ((AppShell)MainPage);
            return mainPage.CurrentPage;
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
            while (popUp.Result == null)
                await Task.Delay(10);

            // in any case close the pop up after receiving a response
            ClosePopUp(popUp);

            return popUp.Result ?? false;
        }
    }
}
