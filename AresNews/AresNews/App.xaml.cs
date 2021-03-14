using AresNews.Models;
using AresNews.Views;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
[assembly: ExportFont("FontAwesome5Free-Regular-400.otf", Alias = "FaRegular")]
[assembly: ExportFont("FontAwesome5Free-Solid-900.otf", Alias = "FaSolid")]
namespace AresNews
{
    public partial class App : Application
    {
        public static Collection<Source> Sources { get; private set; }

        public App()
        {
            Sources = new Collection<Source>();

            InitializeComponent();

            FetchSources();

            MainPage = new NewsPage();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
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
