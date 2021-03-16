using AresNews.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AresNews.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BookmarkPage : ContentPage
    {
        public BookmarkPage()
        {
            InitializeComponent();

            BindingContext = new BookmarkViewModel();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            BindingContext = new BookmarkViewModel();
        }
    }
}