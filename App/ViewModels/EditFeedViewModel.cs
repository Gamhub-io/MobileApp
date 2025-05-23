using CommunityToolkit.Mvvm.Messaging;
using GamHubApp.Core;
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

    private bool? _feedNotificationPrev;
    private bool _feedNotification;
    public bool FeedNotification
    {
        get { return _feedNotification; }
        set
        {
            _feedNotification = value;
            if (_feedNotificationPrev is null)
                _feedNotificationPrev = _feedNotification;
            OnPropertyChanged(nameof(FeedNotification));
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
        await CurrentApp.DataFetcher.UpdateFeed(_feed);

        _context.ListHasBeenUpdated = true;

        if (_initialKeyWords != _feed.Keywords)
            _feed.IsLoaded = false;

        // Close the page
        await App.Current.Windows[0].Page.Navigation.PopAsync();

        System.Collections.ObjectModel.ObservableCollection<Feed> feeds = _context.Feeds;
        _context.CurrentFeedIndex = index = feeds.IndexOf(feeds.FirstOrDefault(feed => feed.Id == _feed.Id));
        _context.FeedTabs[index].Title = _feed.Title;
        WeakReferenceMessenger.Default.Send(new FeedUpdatedMessage(_feed));

        string id = _feed.MongoID;
        string token = await SecureStorage.GetAsync(AppConstant.NotificationToken);
        
        if (string.IsNullOrEmpty(id) || string.IsNullOrEmpty(token))
            return;

        // Update subcription 
        if (_feedNotification)
            await CurrentApp.DataFetcher.SubscribeToFeed(id, token);
        else
            await CurrentApp.DataFetcher.UnsubscribeToFeed(id, token);
    });

    public Microsoft.Maui.Controls.Command Cancel => new Microsoft.Maui.Controls.Command(async () =>
    {
        // Close the page
        await App.Current.Windows[0].Page.Navigation.PopAsync();

    });


    public App CurrentApp { get; }
    public bool IsOnline { get; } = Connectivity.NetworkAccess == NetworkAccess.Internet;

    public EditFeedViewModel(Feed feed, FeedsViewModel vm, GeneralDataBase generalDB)
    {
        _generalDB = generalDB;
        _feed = feed;
        _initialKeyWords = feed.Keywords;
        _context = vm;

        CurrentApp = App.Current as App;

        RefreshFeedSubStatus();

    }
    /// <summary>
    /// Manually refresh the feed subscription status
    /// </summary>
    private async void RefreshFeedSubStatus ()
    {
        string id = _feed.MongoID;
        string token = await SecureStorage.GetAsync(AppConstant.NotificationToken);
        if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(token))
        {
            FeedNotification = await CurrentApp.DataFetcher.CheckSubStatus(id, token);
        }
        return;
    }
}
