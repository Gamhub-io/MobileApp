﻿using GamHub.Models;
using GamHub.ViewModels;
using System;
using CommunityToolkit.Maui.Extensions;
using Microsoft.Maui.Controls.Xaml;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui;
using Microsoft.Maui.Networking;

namespace GamHub.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewsPage : ContentPage
    {
        private NewsViewModel _vm;
        private const int rButtonYStart = -10;
        private double refreshButtonYPos;
        public NewsPage()
        {
            InitializeComponent();

            BindingContext = _vm = new NewsViewModel(this);

            //_vm.IsRefreshing = true;

            // For the auto scroll
            MessagingCenter.Subscribe<MessageItem>(this._vm, "ScrollTop", (sender) =>
            {
                // Scroll to the top of the collection view
                newsCollectionView.ScrollTo(0);
            });
            refreshButtonYPos = refreshButton.Y;
            refreshButton.TranslationY = rButtonYStart;

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();


            _vm.Resume();
        }
        public async void DisplayOfflineMessage(string msg = null)
        {
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {

                await this.DisplayToastAsync($"You're offline: {msg.Replace("[Issue Handler]: ", string.Empty)}", 60000);
                return;
            }
            await this.DisplayToastAsync($"You're offline, please check if you're connected to the internet", 60000);

          
        }
        /// <summary>
        /// Display any message
        /// </summary>
        /// <param name="msg">message to display</param>
        public async Task DisplayMessage(string msg = null)
        {
            await this.DisplayToastAsync(msg, 60000);

          
        }
        /// <summary>
        /// Method allowing the search bar animation
        /// </summary>
        /// <param name="startingHeight">Start size</param>
        /// <param name="endingHeight">End size</param>
        public void AnimateWidthSearchBar(double startingWidth, double endingWidth)
        {
            // update the height of the layout with this call-back
            Action<double> callback = input => { searchBar.WidthRequest = input; };

            // pace at which aniation proceeds
            uint rate = 30;

            // one second animation
            const uint length = 700;
            Easing easing = Easing.Linear;

            searchBar.Animate("invis", callback, startingWidth, endingWidth, rate, length, easing);
        }

        private void OpenSearchButton_Clicked(object sender, EventArgs e)
        {
            OpenSearch();
        }
        /// <summary>
        /// Open the search header
        /// </summary>
        private void OpenSearch()
        {
            AnimateWidthSearchBar(0, 300);

            // Focus on the entry 
            entrySearch.Focus();
        }
        /// <summary>
        /// Close the search header
        /// </summary>
        private void CloseSearch()
        {
            AnimateWidthSearchBar(300, 0);
            _vm.IsSearching = false;
        }

        private void CloseSearchButton_Clicked(object sender, EventArgs e)
        {
            CloseSearch();

            // Close the keyboard 
            entrySearch.Unfocus();
        }

        /// <summary>
        /// Scroll the feed
        /// </summary>
        /// <param name="position">Position you order the feed to be. default 0 (all the way up)</param>
        public void ScrollFeed(int position = 0)
        {
            newsCollectionView.ScrollTo(position);
        }

        protected override bool OnBackButtonPressed()
        {
            if (_vm.IsSearching)
            {
                CloseSearch();
                return true;
            }
            return base.OnBackButtonPressed();
        }

        private void newsCollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
        {
            // FIguring out if the scroll is on top of the screen
            _vm.OnTopScroll = e.FirstVisibleItemIndex == 0;
        }
        /// <summary>
        /// Method to display the refresh button
        /// </summary>
        public void ShowRefreshButton()
        {
            refreshButton.TranslateTo(refreshButton.X, refreshButtonYPos, easing: Easing.BounceOut);
        }
        /// <summary>
        /// Method to remove the refresh button
        /// </summary>
        public void RemoveRefreshButton()
        {
            refreshButton.TranslateTo(refreshButton.X, rButtonYStart);
        }
    }
}