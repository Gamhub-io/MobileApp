using AresNews.Models;
using AresNews.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AresNews.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ArticlePage : ContentPage
    {
        private ArticleViewModel _vm;
        public ArticlePage(Article article)
        {
            InitializeComponent();

            BindingContext = _vm = new ArticleViewModel(article);
        }

        protected override void OnDisappearing()
        {
            // Stop the timer
            _vm.TimeSpent.Stop();
            
            //const int timeToWait = 4;
            //_vm.SelectedArticle.Content = null;

            //// For sefty it's better to wait 4 seconds before navigating back from this page
            //double totalSeconds = _vm.timeSpent.Elapsed.TotalSeconds;

            //if (totalSeconds < timeToWait)
            //{
            //    Thread.Sleep(Convert.ToInt32((timeToWait - totalSeconds)* 1000));
            //}
            base.OnDisappearing();


        }
    }
}