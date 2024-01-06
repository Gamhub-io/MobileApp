using AresNews.Models;
using AresNews.ViewModels;
using AresNews.Views;
using AresNews.Views.PopUps;
using CustardApi.Objects;
using FFImageLoading;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Exceptions;
using Rg.Plugins.Popup.Extensions;
using Rg.Plugins.Popup.Pages;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: ExportFont("FontAwesome6Free-Regular-400.otf", Alias = "FaRegular")]
[assembly: ExportFont("FontAwesome6Brands-Regular-400.otf", Alias = "FaBrand")]
[assembly: ExportFont("FontAwesome6Free-Solid-900.otf", Alias = "FaSolid")]
[assembly: ExportFont("Ubuntu-Regular.ttf", Alias = "P-Regular")]
[assembly: ExportFont("Ubuntu-Bold.ttf", Alias = "P-Bold")]
[assembly: ExportFont("Ubuntu-Medium.otf", Alias = "P-SemiBold")]
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
        public static string ProdHost { get; } = "api.gamhub.io";
        public static string LocalHost { get; } = "gamhubdev.ddns.net";


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
            WService = new Service(host: "gamhubdev.ddns.net",
                                    port: 255,
                                   sslCertificate: false);
#else
        // Set webservice
            WService = new Service(host: ProdHost,
                                   sslCertificate: true);
#endif

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
        public async void ShowLoadingIndicator(bool longer = false)
        {

            try
            {

                if (_isLoading)
                    return;
                _isLoading = true;

                await this.MainPage.Navigation.PushPopupAsync(this.LoadingIndicator);
            }
            catch (RGPageInvalidException)
            {
            }

        }

        /// <summary>
        /// Remove the popup loading indicator
        /// </summary>
        public async void RemoveLoadingIndicator()
        {
            try 
            {
                if (!_isLoading)
                    return;


                _isLoading = false;
                //if (this.MainPage.Navigation.ModalStack.Contains(this.LoadingIndicator))
                await this.MainPage.Navigation.RemovePopupPageAsync(this.LoadingIndicator);
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
    }
}
