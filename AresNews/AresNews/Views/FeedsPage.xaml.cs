using AresNews.ViewModels;
using AresNews.Controls;
using Sharpnado.CollectionView.RenderedViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.CommunityToolkit.UI.Views;

namespace AresNews.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FeedsPage : ContentPage
    {

        private uint _modalHeightStart = 0;
        private uint _modalWidthStart = 50;
        private FeedsViewModel _vm;
        private bool _appeared = false;
        public bool IsFromDetail { get; set; }
        public FeedsPage()
        {
            InitializeComponent();
            BindingContext = _vm = new FeedsViewModel(this);
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();


            _vm.Resume();
            //TabView.SelectedIndexWorkaround = 0;
            //TabView.SelectedIndexWorkaround = _vm.CurrentFeedIndex;
            //SwitchItem(1);

            //TabView.SelectedIndex = _vm.CurrentFeedIndex;

            ////await Task.Factory.StartNew(() =>
            ////{
            //if (_appeared )
            //    BindingContext = _vm = new FeedsViewModel(this);

            //if (_vm.Feeds.Count == 0)
            //    return;
            //ResetTabs();

            //try
            //{
            //    if ( _vm.CurrentFeed == null)
            //    {

            //        await Task.Run(()=> _vm.Refresh(_vm.Feeds[0]));
            //        return;

            //    }
            //    //TabView.TabItemsSource = null;
            //    TabView.SelectedIndex = 0;
            //    //TabView.TabItemsSource = null;

            //    //await Task.Run(() => _vm.Refresh(_vm.Feeds[0]));
            //    //await Task.Run(() => _vm.Refresh(_vm.Feeds[0])).ContinueWith((e) => /*_vm.Refresh(_vm.Feeds[0]*/_vm.Resume()/*_vm.RefreshArticles.Execute(null)*/);



            //}
            //catch
            //{
            //    await Task.Factory.StartNew(() => _vm.Refresh(_vm.Feeds[0]));
            //}
            //_appeared = true;
            ////});
            ///
           // var f = _vm.CurrentFeedIndex;
        }/// <summary>
         /// Function to open a the dropdown
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
        public void ResetTabs ()
        {
            //TabView.TabItems.Clear();
            //TabView.TabItems = new();

            //foreach (var item in _vm.Feeds)
            //{
            //    TabView.TabItems.Add(new()
            //    {
            //        Text = item.Title
            //    });
            //}
            //TabView.SelectedIndex = 0;
        }
        /// <summary>
        /// Remove a tab organically 
        /// </summary>
        /// <param name="index">index of the tab you want to remove</param>
        //public void RemoveTab (int index)
        //{
        //    TabView.TabItems.RemoveAt(index);
        //}

        //private void TabView_ChildRemoved(object sender, ElementEventArgs e)
        //{

        //}
        //private void SwitchItem(int index)
        //{
        //    if (index != -1 && index < TabView.TabItems.Count)
        //    {
        //        // See: https://github.com/xamarin/XamarinCommunityToolkit/issues/595
        //        MethodInfo dynMethod = TabView.GetType().GetMethod("UpdateSelectedIndex", BindingFlags.NonPublic | BindingFlags.Instance);
        //        dynMethod?.Invoke(TabView, new object[] { index, false });
        //    }
        //}

        private void TabView_SelectionChanged(object sender, TabSelectionChangedEventArgs e)
        {
            //var s = (sender as TabViewWorkaround);
            //if (_vm.IsFromDetail && (s.SelectedIndexWorkaround != _vm.CurrentFocusIndex))
            //{
            //    s.SelectedIndexWorkaround = _vm.CurrentFocusIndex;
            //    return;
            //};
            //_vm.CurrentFocusIndex = s.SelectedIndexWorkaround;

           // _vm.IsFromDetail = false;


        }
    }
}