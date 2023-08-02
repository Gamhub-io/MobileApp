using AresNews.Models;
using AresNews.Views;
using MvvmHelpers;
using Rg.Plugins.Popup.Extensions;

namespace AresNews.ViewModels
{
    public class EditFeedViewModel : BaseViewModel
    {
        private Feed _feed;
        private string _initialKeyWords;

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
        private RenameFeedPopUp _page;
        private int index;

        public RenameFeedPopUp Page
        {
            get { return _page; }
            set
            {
                _page = value;
                OnPropertyChanged(nameof(Feed));
            }
        }
        public Xamarin.Forms.Command Validate => new Xamarin.Forms.Command(async () =>
        {

            // Remove feed
            Context.UpdateCurrentFeed(_feed);
            // update the feed
            App.SqLiteConn.Update(_feed);

            _context.ListHasBeenUpdated = true;

            if (_initialKeyWords != _feed.Keywords)
                _feed.IsLoaded = false;

            // Close the page
            await App.Current.MainPage.Navigation.PopAsync();

            _context.CurrentFeedIndex = index = _context.Feeds.IndexOf(_feed);
            _context.FeedTabs[index].Title = _feed.Title;

            //_context.UpdateOrders.Add(new UpdateOrder
            //{
            //    Feed = _feed,
            //    Update = UpdateOrder.FeedUpdate.Edit
                
            //});
        });

        public Xamarin.Forms.Command Cancel => new Xamarin.Forms.Command(async () =>
        {
            // Close the page

            //_context.CurrentFeedIndex = _context.Feeds.IndexOf(_feed);
            await App.Current.MainPage.Navigation.PopAsync();

        });
        public EditFeedViewModel( Feed feed, FeedsViewModel vm )
        {
            _feed = feed;
            _initialKeyWords = feed.Keywords;
            _context = vm;
        }
    }
}
