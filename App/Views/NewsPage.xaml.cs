﻿using GamHubApp.ViewModels;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.Messaging;
using GamHubApp.Services;
using GamHubApp.Services.ChangedMessages;

namespace GamHubApp.Views;

[XamlCompilation(XamlCompilationOptions.Compile)]
public partial class NewsPage : ContentPage
{
    private NewsViewModel _vm;
    private const int rButtonYStart = -10;
    private const int _searchbarEndingWidth = 270;
    private double refreshButtonYPos;
    
    public NewsPage(NewsViewModel vm)
    {
        InitializeComponent();

        BindingContext = _vm = vm;

        refreshButton.TranslationY = rButtonYStart;

        WeakReferenceMessenger.Default.Register<UnnoticedArticlesChangedMessage>(this, (r, m) =>
        {
            if (m.Count > 0)
                ShowRefreshButton();
            else
                RemoveRefreshButton();
        });

        WeakReferenceMessenger.Default.Register<ScrollMainPageChangedMessage>(this, (r, m) =>
        {
            ScrollFeed();
        });

    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        OnResume();
    }

    public void OnResume()
    {
        _vm.Resume().GetAwaiter();
    }

    public async void DisplayOfflineMessage(string msg = null)
    {
        var current = Connectivity.NetworkAccess;
        string message = "You're offline, please check if you're connected to the internet";
        if (current == NetworkAccess.Internet)
            message = $"You're offline: {msg.Replace("[Issue Handler]: ", string.Empty)}";

        await Snackbar.Make(message).Show();

      
    }
    /// <summary>
    /// Display any message
    /// </summary>
    /// <param name="msg">message to display</param>
    public async Task DisplayMessage(string msg = null)
    {
        await Snackbar.Make(msg).Show();
    }
    /// <summary>
    /// Method allowing the search bar animation
    /// </summary>
    /// <param name="startingWidth">Start size</param>
    /// <param name="endingWidth">End size</param>
    public void AnimateWidthSearchBar(double startingWidth, double endingWidth)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // update the height of the layout with this call-back
            Action<double> callback = input => { searchBar.WidthRequest = input; };

            // pace at which animation proceeds
            uint rate = 30;

            // one second animation
            const uint length = 700;
            Easing easing = Easing.Linear;

            searchBar.Animate("invis", callback, startingWidth, endingWidth, rate, length, easing);
        });
    }

    private void OpenSearchButton_Clicked(object sender, EventArgs e)
    {
        OpenSearch();
    }
    /// <summary>
    /// Open the search header
    /// </summary>
    private void OpenSearch()
    {
        AnimateWidthSearchBar(0, _searchbarEndingWidth);

        // Focus on the entry 
        entrySearch.Focus();
    }
    /// <summary>
    /// Close the search header
    /// </summary>
    private void CloseSearch()
    {
        AnimateWidthSearchBar(_searchbarEndingWidth, 0);
        _vm.IsSearching = false;
    }

    private void CloseSearchButton_Clicked(object sender, EventArgs e)
    {
        CloseSearch();

        // Close the keyboard 
        entrySearch.Unfocus();
    }

    /// <summary>
    /// Scroll the feed
    /// </summary>
    /// <param name="position">Position you order the feed to be. default 0 (all the way up)</param>
    public void ScrollFeed(int position = 0)
    {
        newsCollectionView.ScrollTo(position);
    }

    protected override bool OnBackButtonPressed()
    {
        if (_vm.IsSearching)
        {
            CloseSearch();
            return true;
        }
        return base.OnBackButtonPressed();
    }

    private void newsCollectionView_Scrolled(object sender, ItemsViewScrolledEventArgs e)
    {
        // FIguring out if the scroll is on top of the screen
        _vm.OnTopScroll = e.FirstVisibleItemIndex == 0;
    }

    /// <summary>
    /// Method to display the refresh button
    /// </summary>
    public void ShowRefreshButton()
    {
        refreshButton.TranslateTo(0, refreshButtonYPos, easing: Easing.BounceOut);
    }

    /// <summary>
    /// Method to remove the refresh button
    /// </summary>
    public void RemoveRefreshButton()
    {
        refreshButton.TranslateTo(0, refreshButtonYPos);
    }
}