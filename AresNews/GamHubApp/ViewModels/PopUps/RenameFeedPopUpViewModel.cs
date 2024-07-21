using GamHub.Models;
using GamHub.Views;

namespace GamHub.ViewModels.PopUps
{
    public class RenameFeedPopUpViewModel : BaseViewModel
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
        private RenameFeedPopUp _popUp;

        public RenameFeedPopUp PopUp
        {
            get { return _popUp; }
            set
            {
                _popUp = value;
                OnPropertyChanged(nameof(PopUp));
            }
        }

        public App CurrentApp { get; }
        public Microsoft.Maui.Controls.Command Validate => new Microsoft.Maui.Controls.Command(() =>
        {

            // Remove feed
            Context.UpdateCurrentFeed(_feed);
            // update the feed
            App.SqLiteConn.Update(_feed);

            // Close the popup
            _popUp.Close();
        });

        public Microsoft.Maui.Controls.Command Cancel => new Microsoft.Maui.Controls.Command(() =>
        {
            // Close the popup
            _popUp.Close();
        });
        public RenameFeedPopUpViewModel(RenameFeedPopUp page, Feed feed, FeedsViewModel vm )
        {
            _feed = feed;
            _popUp = page;
            _context = vm;

            CurrentApp = App.Current as App;
        }
    }
}
