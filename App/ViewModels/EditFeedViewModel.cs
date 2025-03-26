using CommunityToolkit.Mvvm.Messaging;
using GamHubApp.Models;
using GamHubApp.Services;
using GamHubApp.Views;

namespace GamHubApp.ViewModels;

public class EditFeedViewModel : BaseViewModel
{
    private GeneralDataBase _generalDB;
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
    public Microsoft.Maui.Controls.Command Validate => new Microsoft.Maui.Controls.Command(async () =>
    {

        // Update feed
        Context.UpdateCurrentFeed(_feed);
        await _generalDB.UpdateFeed(_feed);

        _context.ListHasBeenUpdated = true;

        if (_initialKeyWords != _feed.Keywords)
            _feed.IsLoaded = false;

        // Close the page
        await App.Current.Windows[0].Page.Navigation.PopAsync();

        System.Collections.ObjectModel.ObservableCollection<Feed> feeds = _context.Feeds;
        _context.CurrentFeedIndex = index = feeds.IndexOf(feeds.FirstOrDefault(feed => feed.Id == _feed.Id));
        _context.FeedTabs[index].Title = _feed.Title;
        WeakReferenceMessenger.Default.Send(new FeedUpdatedMessage(_feed));

    });

    public Microsoft.Maui.Controls.Command Cancel => new Microsoft.Maui.Controls.Command(async () =>
    {
        // Close the page
        await App.Current.Windows[0].Page.Navigation.PopAsync();

    });
    public EditFeedViewModel(Feed feed, FeedsViewModel vm, GeneralDataBase generalDB)
    {
        _generalDB = generalDB;
        _feed = feed;
        _initialKeyWords = feed.Keywords;
        _context = vm;
    }
}
