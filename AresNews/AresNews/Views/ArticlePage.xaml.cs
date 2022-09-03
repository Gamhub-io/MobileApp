using AresNews.Models;
using AresNews.ViewModels;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AresNews.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ArticlePage : ContentPage
    {
        private ArticleViewModel _vm;
        private uint _modalHeightStart = 0;


        public ArticlePage(Article article)
        {
            InitializeComponent();

            BindingContext = _vm = new ArticleViewModel(article);
        }

        protected override void OnDisappearing()
        {
            // Stop the timer
            _vm.TimeSpent.Stop();

            // Stop all text to speech
            _vm.StopTtS();


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
        /// <summary>
        /// Function to open a the dropdrown
        /// </summary>
        public void OpenDropdownMenu()
        {
            double height = 70;
            //_vm.ModalClose = false;
            // Animation
            void callback(double input) => dropdownMenu.HeightRequest = input;

            uint rate = 24;
            dropdownMenu.Animate("AnimDropdownMenu", callback, dropdownMenu.Height, height, rate, 100, Easing.SinOut);
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
            void callback(double input) => dropdownMenu.HeightRequest = input;
            uint rate = 24;
            dropdownMenu.Animate("AnimDropdownMenu", callback, dropdownMenu.Height, _modalHeightStart, rate, 500, Easing.SinOut);
            dropdownMenu.Padding = 0;

            _vm.IsMenuOpen= false;
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