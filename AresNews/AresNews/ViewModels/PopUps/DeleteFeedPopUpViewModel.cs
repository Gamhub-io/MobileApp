using AresNews.Models;
using AresNews.Views;
using MvvmHelpers;
using Rg.Plugins.Popup.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

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
        private DeleteFeedPopUp _context;

        public DeleteFeedPopUp Context
        {
            get { return _context; }
            set 
            {
                _context = value;
                OnPropertyChanged(nameof(Feed));
            }
        }

        public Xamarin.Forms.Command Delete => new Xamarin.Forms.Command(async() =>
        {
            // Delete the feed
            App.SqLiteConn.Delete(_feed);

            // Close the popup
            await _context.Navigation.RemovePopupPageAsync(_context);
        });

        public Xamarin.Forms.Command Cancel => new Xamarin.Forms.Command(async() =>
        {
            // Close the popup
            await _context.Navigation.RemovePopupPageAsync(_context);
        });

        public DeleteFeedPopUpViewModel(DeleteFeedPopUp ctx, Feed feed)
        {
            Context = ctx;
            Feed = feed;

        }
    }
}
