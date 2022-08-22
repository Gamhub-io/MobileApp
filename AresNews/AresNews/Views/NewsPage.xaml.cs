using AresNews.Models;
using AresNews.ViewModels;
using MvvmHelpers;
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
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.CommunityToolkit.Extensions;
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

            BindingContext = _vm = new NewsViewModel(this);

            //_vm.IsRefreshing = true;

            // For the auto scroll
            MessagingCenter.Subscribe<MessageItem>(this._vm, "ScrollTop", (sender) =>
            {
                // Scroll to the top of the collection view
                newsCollectionView.ScrollTo(0);
            });

        }
        protected override void OnAppearing()
        {
            base.OnAppearing();

            //_vm.FetchArticles();

            // Is the app connected to the internet
            //if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            //{
            //    this.DisplayToastAsync("You're offline", 60000);
            //}
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
        /// Method allowing the searchbar annimation
        /// </summary>
        /// <param name="startingHeight">Start size</param>
        /// <param name="endingHeight">End size</param>
        public void AnimateWidthSearchBar(double startingWidth, double endingWidth)
        {
            // update the height of the layout with this callback
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
            AnimateWidthSearchBar(0, 300);

            // Focus on the entry 
            entrySearch.Focus();
        }

        private void CloseSearchButton_Clicked(object sender, EventArgs e)
        {
            AnimateWidthSearchBar(300, 0);
        }
        /// <summary>
        /// Scroll the feed
        /// </summary>
        /// <param name="position">Position you order the feed to be. default 0 (all the way up)</param>
        public void ScrollFeed(int position = 0)
        {
            newsCollectionView.ScrollTo(position);
        }
    }
}