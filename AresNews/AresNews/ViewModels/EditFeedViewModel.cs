using AresNews.Models;
using AresNews.Views;
using MvvmHelpers;
using Rg.Plugins.Popup.Extensions;

namespace AresNews.ViewModels
{
    public class EditFeedViewModel : BaseViewModel
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
        private RenameFeedPopUp _page;

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



            // Close the page
            await App.Current.MainPage.Navigation.PopAsync();

            _context.CurrentFeedIndex = _context.Feeds.IndexOf(_feed);
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
            _context = vm;
        }
    }
}
