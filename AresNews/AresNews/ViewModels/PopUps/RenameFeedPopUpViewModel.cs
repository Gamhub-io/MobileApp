using AresNews.Models;
using AresNews.Views;
using MvvmHelpers;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace AresNews.ViewModels.PopUps
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



            // Close the popup
            await App.Current.MainPage.Navigation.RemovePopupPageAsync(_page);
        });

        public Xamarin.Forms.Command Cancel => new Xamarin.Forms.Command(async () =>
        {
            // Close the popup
            await App.Current.MainPage.Navigation.RemovePopupPageAsync(_page);
        });
        public RenameFeedPopUpViewModel(RenameFeedPopUp page, Feed feed, FeedsViewModel vm )
        {
            _feed = feed;
            _page = page;
            _context = vm;
        }
    }
}
