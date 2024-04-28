using AresNews.Models;
using AresNews.Services;
using AresNews.ViewModels;
using AresNews.Views;
using AresNews.Core;
using AresNews.Views.PopUps;
using CustardApi.Objects;
using Rg.Plugins.Popup.Exceptions;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Essentials;
using Newtonsoft.Json;

namespace AresNews
{
    public partial class App : Application
    {
        private bool _isLoading;

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

            Sharpnado.Tabs.Initializer.Initialize(false, false);
            Sharpnado.Shades.Initializer.Initialize(loggerEnable: false);
            Sources = new Collection<Source>();

            InitializeComponent();
            Sharpnado.CollectionView.Initializer.Initialize(true, false);

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

            MainPage =  new AppShell();

            
        }
        /// <summary>
        /// Show the popup loading indicator
        /// </summary>
        public void ShowLoadingIndicator(bool longer = false)
        {

            try
            {

                if (_isLoading)
                    return;
                _isLoading = true;

                OpenPopUp(this.LoadingIndicator);
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
                if (!_isLoading)
                    return;


                _isLoading = false;
                
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
        public static async void StartDb()
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
        }

        protected override void OnSleep()
        {
            base.OnSleep();

            AppShell mainPage = ((AppShell)MainPage);
            Page currentPage = mainPage.CurrentPage;

            if (currentPage.ToString() == "AresNews.Views.ArticlePage")
            {

                ((ArticleViewModel)((ArticlePage)currentPage).BindingContext).TimeSpent.Stop();
            }
            //SqLiteConn.Dispose();
        }
        

        protected override void OnResume()
        {
            AppShell mainPage = ((AppShell)MainPage);
            Page currentPage = mainPage.CurrentPage;

            if (currentPage.ToString() == "AresNews.Views.ArticlePage")
            {

                ((ArticleViewModel)((ArticlePage)currentPage).BindingContext).TimeSpent.Start();
            }
            //else if (currentPage.ToString() == "AresNews.Views.NewPage")
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
        public async void OpenPopUp(PopupPage popUp, Page page = null)
        {
            try
            {

                if (popUp == null)
                    return;

                if (page == null)
                    page = GetCurrentPage();

                await page.Navigation.PushPopupAsync(popUp);
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
        public async void ClosePopUp(PopupPage popUp, Page page = null)
        {
            try
            {

                if (popUp == null)
                    return;

                if (page == null)
                    page = GetCurrentPage();

                await page.Navigation.RemovePopupPageAsync(popUp);
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
    }
}
