using AresNews.Models;
using AresNews.ViewModels;
using AresNews.Views;
using CustardApi.Objects;
using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
[assembly: ExportFont("FontAwesome5Free-Regular-400.otf", Alias = "FaRegular")]
[assembly: ExportFont("FontAwesome5Brands-Regular-400.otf", Alias = "FaBrand")]
[assembly: ExportFont("FontAwesome5Free-Solid-900.otf", Alias = "FaSolid")]
[assembly: ExportFont("public-sans.regular.otf", Alias = "P-Regular")]
[assembly: ExportFont("public-sans.bold.otf", Alias = "P-Bold")]
[assembly: ExportFont("public-sans.semibold.otf", Alias = "P-SemiBold")]
namespace AresNews
{
    public partial class App : Application
    {
        public static Collection<Source> Sources { get; private set; }
        // Property SqlLite Connection
        public static SQLiteConnection SqLiteConn { get; set; }
        public static Service WService { get; set; }


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
            // Set webservice
            WService = new Service(host: "pinnate-beautiful-marigold.glitch.me",
                                   sslCertificate: true);

            Sources = new Collection<Source>();

            InitializeComponent();

            FetchSources();

            

            // Start the db
            StartDb();

            SqLiteConn.CreateTable<Article>();

            // Close the db
            //CloseDb();

            MainPage = new AppShell();
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
            // Just use whatever directory SpecialFolder.Personal returns
            string libraryPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

            var path = Path.Combine(libraryPath, "ares.db3");

            // Verify if a data base already exist
            if (!File.Exists(path))
                // Create the folder path.
                File.Create(path);
            
            // Sqlite connection
            SqLiteConn = new SQLiteConnection(path);

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

                ((ArticleViewModel)((ArticlePage)currentPage).BindingContext).timeSpent.Stop();
            }
            //SqLiteConn.Dispose();
        }
        

        protected override void OnResume()
        {
            AppShell mainPage = ((AppShell)MainPage);
            Page currentPage = mainPage.CurrentPage;

            if (currentPage.ToString() == "AresNews.Views.ArticlePage")
            {

                ((ArticleViewModel)((ArticlePage)currentPage).BindingContext).timeSpent.Start();
            }

            StartDb();
        }
        /// <summary>
        /// Fetch all the sources
        /// </summary>
        public void FetchSources()
        {
            var assembly = typeof(App).GetTypeInfo().Assembly;

            var res = assembly.GetManifestResourceNames().FirstOrDefault(r => r == "AresNews.Resources.Sources.json");

            var stream = assembly.GetManifestResourceStream(res);

            using (var reader = new System.IO.StreamReader(stream))
            {

                var json = reader.ReadToEnd();
                Sources = JsonConvert.DeserializeObject<Collection<Source>>(json);
            }
        }
    }
}
