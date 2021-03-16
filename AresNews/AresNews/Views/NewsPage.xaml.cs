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
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AresNews.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsPage : ContentPage
    {
        public NewsPage()
        {
            InitializeComponent();

            BindingContext = new NewsViewModel ();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            BindingContext = new NewsViewModel();
        }
    }
}