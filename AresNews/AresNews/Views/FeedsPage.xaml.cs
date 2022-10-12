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
    public partial class FeedsPage : ContentPage
    {
        private FeedsViewModel _vm;

        public FeedsPage()
        {
            InitializeComponent();
            BindingContext = _vm = new FeedsViewModel();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            _vm.Refresh(_vm.Feeds[0]);
        }
    }
}