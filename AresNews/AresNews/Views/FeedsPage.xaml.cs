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

        private uint _modalHeightStart = 0;
        private uint _modalWidthStart = 50;
        private FeedsViewModel _vm;
        private bool _appeared = false;
        public FeedsPage()
        {
            InitializeComponent();
            BindingContext = _vm = new FeedsViewModel(this);
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            //await Task.Factory.StartNew(() =>
            //{
            if (_appeared )
                BindingContext = _vm = new FeedsViewModel(this);
            
            if (_vm.Feeds.Count == 0)
                return;
            try
            {
                if ( _vm.CurrentFeed == null)
                {

                    await Task.Factory.StartNew(()=> _vm.Refresh(_vm.Feeds[0]));
                    return;

                }
                await Task.Factory.StartNew(() => _vm.Refresh(_vm.Feeds[0])).ContinueWith((e) => /*_vm.Refresh(_vm.Feeds[0]*/_vm.Resume()/*_vm.RefreshArticles.Execute(null)*/);
                
                

            }
            catch
            {
                await Task.Factory.StartNew(() => _vm.Refresh(_vm.Feeds[0]));
            }
            _appeared = true;
            //});
        }/// <summary>
         /// Function to open a the dropdrown
         /// </summary>
        public void OpenDropdownMenu()
        {
            double height = 70;
            double width = 180;
            //_vm.ModalClose = false;
            // Animation
            void callbackH(double inputH) => dropdownMenu.HeightRequest = inputH;
            void callbackW(double inputW) => dropdownMenu.WidthRequest = inputW;

            uint rate = 24;
            dropdownMenu.Animate("AnimHeightDropdownMenu", callbackH, dropdownMenu.Height, height, rate, 100, Easing.SinOut);
            dropdownMenu.Animate("AnimWidthDropdownMenu", callbackW, dropdownMenu.Width, width, rate, 100, Easing.SinOut);
            dropdownMenu.Padding = 3;

            _vm.IsMenuOpen = true;
        }
        /// <summary>
        /// Function to close a modal
        /// </summary>
        public void CloseDropdownMenu()
        {
            //_vm.ModalClose = true;

            // Animation
            void callbackH(double inputH) => dropdownMenu.HeightRequest = inputH;
            void callbackW(double inputW) => dropdownMenu.WidthRequest = inputW;
            uint rate = 24;

            dropdownMenu.Animate("AnimWidthDropdownMenu", callbackW, dropdownMenu.Width, _modalWidthStart, rate, 500, Easing.SinOut);
            dropdownMenu.Animate("AnimHeightDropdownMenu", callbackH, dropdownMenu.Height, _modalHeightStart, rate, 500, Easing.SinOut);

            dropdownMenu.Padding = 0;

            _vm.IsMenuOpen = false;
        }

        private void Menu_Clicked(object sender, EventArgs e)
        {
            // If dropdown is closed
            if (dropdownMenu.Padding == 0)
            {
                OpenDropdownMenu();
                return;
            }

            CloseDropdownMenu();
        }

        private void MenuItem_Tapped(object sender, EventArgs e)
        {
            // If dropdown is open
            if (dropdownMenu.Padding != 0)
            {
                CloseDropdownMenu();
            }
        }
    }
}