using AresNews.Models;
using AresNews.Views;
using MvvmHelpers;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;

namespace AresNews.ViewModels.PopUps
{
    public class DeleteFeedPopUpViewModel : BaseViewModel
    {
        private Feed _feed;

        public Feed Feed
        {
            get { return _feed; }
            set 
            {
                _feed = value;
                OnPropertyChanged(nameof(Feed));
            }
        }
        private FeedsViewModel _context;

        public FeedsViewModel Context
        {
            get { return _context; }
            set 
            {
                _context = value;
                OnPropertyChanged(nameof(Feed));
            }
        }
        private DeleteFeedPopUp _page;

        public DeleteFeedPopUp Page
        {
            get { return _page; }
            set 
            {
                _page = value;
                OnPropertyChanged(nameof(Feed));
            }
        }

        public Xamarin.Forms.Command Delete => new Xamarin.Forms.Command(async() =>
        {

            // Delete the feed
            App.SqLiteConn.Delete(_feed);

            int index = _context.Feeds.IndexOf(_feed);
            _context.FeedTabs.RemoveAt(index);
            _context.Feeds.RemoveAt(index);
            _context.SelectDefaultTab(index);
            //_context.Refresh(_context.Feeds[index-1]);

            // Update the feeds remotely
            //MessagingCenter.Send<Feed>(_feed, "RemoveFeed");

            // Close the popup
            await Application.Current.MainPage.Navigation.RemovePopupPageAsync(_page);
        });

        public Xamarin.Forms.Command Cancel => new Xamarin.Forms.Command(async() =>
        {
            // Close the popup
            await App.Current.MainPage.Navigation.RemovePopupPageAsync(_page);
        });

        public DeleteFeedPopUpViewModel(DeleteFeedPopUp page, Feed feed, FeedsViewModel ctx)
        {
            Context = ctx;
            Page = page;
            Feed = feed;


        }
    }
}
