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
        public Xamarin.Forms.Command Validate => new Xamarin.Forms.Command(() =>
        {

            // Remove feed
            Context.UpdateCurrentFeed(_feed);
            // update the feed
            App.SqLiteConn.Update(_feed);

            // Close the popup
            CurrentApp.ClosePopUp (_popUp);
        });

        public Xamarin.Forms.Command Cancel => new Xamarin.Forms.Command(() =>
        {
            // Close the popup
            CurrentApp.ClosePopUp (_popUp);
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
