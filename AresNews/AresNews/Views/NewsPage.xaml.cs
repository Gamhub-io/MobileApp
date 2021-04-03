using AresNews.Models;
using AresNews.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AresNews.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsPage : ContentPage
    {
        private NewsViewModel _vm;
        public NewsPage()
        {
            InitializeComponent();

            BindingContext = _vm = new NewsViewModel ();

            _vm.IsRefreshing = true;

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            _vm.RefreshArticles();
        }
    }
}