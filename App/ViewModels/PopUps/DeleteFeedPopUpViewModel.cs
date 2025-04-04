﻿using GamHubApp.Models;
using GamHubApp.Services;
using GamHubApp.Views;
using SQLite;

namespace GamHubApp.ViewModels.PopUps
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
        private GeneralDataBase _generalDB;

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
        public App CurrentApp { get; }

        public Microsoft.Maui.Controls.Command Delete => new Microsoft.Maui.Controls.Command(async () =>
        {
            await _generalDB.DeleteFeed(_feed);

            int index = _context.Feeds.IndexOf(_feed);

            // Remove the feed from the local DB
            _context.FeedTabs.RemoveAt(index);

            // Remove the feed from the feed page
            _context.RemoveFeedByIndex(index);

            // Close the popup
            _page.Close();
        });

        public Microsoft.Maui.Controls.Command Cancel => new Microsoft.Maui.Controls.Command(() =>
        {
            // Close the popup
            _page.Close();
        });

        public DeleteFeedPopUpViewModel(DeleteFeedPopUp page, Feed feed, FeedsViewModel ctx, GeneralDataBase generalDataBase )
        {
            _generalDB = generalDataBase;
            Context = ctx;
            Page = page;
            Feed = feed;
            CurrentApp = App.Current as App;


        }
    }
}
