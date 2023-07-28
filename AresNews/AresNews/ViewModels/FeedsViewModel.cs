using AresNews.Helpers.Tools;
using AresNews.Models;
using AresNews.Views;
using MvvmHelpers;
using Rg.Plugins.Popup.Extensions;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.PancakeView;
using static AresNews.Models.UpdateOrder;
using Command = Xamarin.Forms.Command;

namespace AresNews.ViewModels
{
    public class FeedsViewModel : BaseViewModel
    {
        
        public List<UpdateOrder> UpdateOrders { get; private set; }
        private ObservableCollection<Feed> _feeds;

		public ObservableCollection<Feed> Feeds
		{
			get { return _feeds; }
			set 
			{
				_feeds = value; 
				OnPropertyChanged(nameof(Feeds));
			}
		}
        private ObservableCollection<TabButton> _feedTabs;

		public ObservableCollection<TabButton> FeedTabs
		{
			get { return _feedTabs; }
			set 
			{
                _feedTabs = value; 
				OnPropertyChanged(nameof(FeedTabs));
			}
		}
        private int _currentFeedIndex;

        public int CurrentFeedIndex
        {
            get { return _currentFeedIndex; }
            set 
            {
                _currentFeedIndex = value;
                SwitchTabs(_currentFeedIndex);

                OnPropertyChanged(nameof(CurrentFeedIndex));

                _oldIndex = _currentFeedIndex;
            }
        }


        private bool _isRefreshing; 


        public bool IsRefreshing 
		{
			get { return _isRefreshing; }
			set 
			{
                _isRefreshing = value; 
				OnPropertyChanged(nameof(IsRefreshing));
			}
		}
		private ObservableCollection<Article> _articles;

		public ObservableCollection<Article> Articles
		{
			get { return _articles; }
			set 
			{
                _articles = value; 
				OnPropertyChanged(nameof(Articles));
			}
		}
        private Feed _selectedFeed;

        public Feed SelectedFeed
        {
            get { return _selectedFeed; }
            set 
            {
                _selectedFeed = value;
                CurrentFeedIndex = _feeds.IndexOf(value);
                Refresh(value);
                OnPropertyChanged(nameof(SelectedFeed));
            }
        }
        public Xamarin.Forms.Command<Feed> RefreshAll => new Command<Feed>((feed) =>
        {
            IsRefreshing = true;
            Task.Run(() =>
            {
                this.Refresh(feed);
            });

        });

        public Xamarin.Forms.Command<Feed> SwitchFeed => new Command<Feed>(async (feed) =>
        {
            if (IsBusy)
                return;
            //IsRefreshing = true;

            CurrentApp.ShowLoadingIndicator();
            await Task.Run(() =>
             {

                 // Set new feed
                 //SelectedFeed = feed;
                 SwitchTabs(_feeds.IndexOf(feed));
                 SelectedFeed.IsLoaded = false;

                 //// Load articles
                 //
                 

             });

            RefreshArticles.Execute(null);
            CurrentApp.RemoveLoadingIndicator();

        });

        public App CurrentApp { get; }
        private FeedsPage CurrentPage { get; set; }

        public Xamarin.Forms.Command<Feed> Delete => new Xamarin.Forms.Command<Feed>(async (feed) =>
        {
            await CurrentPage.Navigation.PushPopupAsync(new DeleteFeedPopUp(_selectedFeed, this));

        });

        public Xamarin.Forms.Command<Feed> Rename => new Xamarin.Forms.Command<Feed>(async (feed) =>
        {
            await CurrentPage.Navigation.PushPopupAsync(new RenameFeedPopUp(_selectedFeed, this));

        });

        public Xamarin.Forms.Command<Feed> Edit => new Xamarin.Forms.Command<Feed>(async (feed) =>
        {
            IsFromDetail = true;
            CurrentFocusIndex = _feeds.IndexOf(_selectedFeed);
            await CurrentPage.Navigation.PushAsync(new EditFeedPage(_selectedFeed, this));

        });

        public Xamarin.Forms.Command RefreshArticles => new Xamarin.Forms.Command(() =>
        {


            if (IsBusy)
                return;
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (_feeds.Count <= 0)
                    return;
                //IsRefreshing = true;
                await Task.Run(() =>
                {
                    try
                    {

                        if (SelectedFeed == null)
                        {
                            SelectedFeed = _feeds[0];

                        }
                        this.Refresh(SelectedFeed);
                    }
                    catch
                    {
                        // in case of error, refresh the first feed
                        this.Refresh(_feeds[0]);
                    }
                });
            });
        });
		public FeedsViewModel(FeedsPage page)
        {
            // CurrentApp and CurrentPage will allow use to access to global properties
            CurrentApp = App.Current as App;
            CurrentPage = page;
            
            // Instantiate definitions 
            FeedTabs = new ObservableCollection<TabButton>();
            Feeds = new ObservableCollection<Feed>(App.SqLiteConn.GetAllWithChildren<Feed>());

            // Organise feeds into tabs
            CopyFeedsToTabs();

            // Select first item
            CurrentFeedIndex = 0;

            

            UpdateOrders = new List<UpdateOrder>();


            if (_feeds.Count <= 0)
                return;

            IsRefreshing = true;

            RefreshArticles.Execute(null);


            MessagingCenter.Subscribe<Feed>(this, "AddFeed", (sender) =>
            {
                //Feeds.Add(sender);
                // Add the new item to the separate collection
                //_feeds.Add(sender);

                //// Update the ItemsSource of the CarouselView with the new collection
                //Feeds = new ObservableCollection<Feed>(_feeds);

                UpdateOrders.Add(new UpdateOrder
                {
                    Update= UpdateOrder.FeedUpdate.Add,
                    Feed= sender
                });

            });

            MessagingCenter.Subscribe<Feed>(this, "RemoveFeed", (sender) =>
            {

                if (sender == null)
                    return;

                UpdateOrders.Add(new UpdateOrder
                {
                    Update = UpdateOrder.FeedUpdate.Remove,
                    Feed = sender
                });


            });

            MessagingCenter.Subscribe<Feed>(this, "EditFeed", (sender) =>
            {

                if (sender == null)
                    return;

                UpdateOrders.Add(new UpdateOrder
                {
                    Update = UpdateOrder.FeedUpdate.Edit,
                    Feed = sender
                });


            });

            // Set command to share an article
            _shareArticle = new Command(async (id) =>
            {
                // Get selected article
                var article = Articles.FirstOrDefault(art => art.Id == id.ToString());

                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = article.Url,
                    Title = "Share this article",
                    Subject = article.Title,
                    Text = article.Title
                });
            });
            _addBookmark = new Command((id) =>
            {
                var article = new Article();
                // Get the article
                article = Articles.FirstOrDefault(art => art.Id == id.ToString());



                // If the article is already in bookmarks
                bool isSaved = article.IsSaved;

                //// Marked the article as saved
                article.IsSaved = !article.IsSaved;

                if (isSaved)
                    App.SqLiteConn.Delete(article, recursive: true);
                else
                {
                    // Insert it in database
                    App.SqLiteConn.InsertWithChildren(article, recursive: true);
                }



                //Articles[_articles.IndexOf(article)] = article;

                // Say the the bookmark has been updated
                MessagingCenter.Send<Article>(article, "SwitchBookmark");


            });
        }
        private readonly Command _shareArticle;

        public Command ShareArticle
        {
            get { return _shareArticle; }
        }

        public Command FeedSelect
        {
            get { return new Command<string> ((feedId) =>
            {

                Feed nextFeed = Feeds.FirstOrDefault(feed => feed.Id == feedId);
                int nextIndex = _feeds.IndexOf(nextFeed);

                if (nextIndex == -1)
                    return;
                // Change the button colour of the clicked item
                FeedTabs[nextIndex].BackgroundColour = (Color)Application.Current.Resources["PrimaryAccent"];

                // Reset the button colour for the previously selected item (if any)
                if (SelectedFeed != null && SelectedFeed != nextFeed)
                {
                    FeedTabs[nextIndex].BackgroundColour = (Color)Application.Current.Resources["LightDark"];
                }

                SelectedFeed = nextFeed;

                // Flag all the other feeds as unloaded
                for (int i = 0; i < _feeds.Count ; i++)
                {
                    if (_feeds[i].Id != SelectedFeed.Id)
                        _feeds[i].IsLoaded = false;
                }

            }); }
        }
        // Command to add a Bookmark
        private readonly Command _addBookmark;

        public Command AddBookmark
        {
            get
            {
                return _addBookmark;
            }
        }
        public async void Refresh(Feed feed)
		{
            if (IsBusy)
                return;
            IsBusy = true;

            CurrentApp.ShowLoadingIndicator();
                bool isFirstLoad = feed != _selectedFeed;
                if (isFirstLoad)
                {

                    SelectedFeed = feed;
                    SelectedFeed.IsLoaded = false;
                }


                await Task.Run(() =>
                {

                    AggregateFeed(feed, isFirstLoad);

                    // End the loading indicator
                    IsRefreshing = false;
                    IsBusy = false;
                    CurrentApp.RemoveLoadingIndicator();

                });
        }
        /// <summary>
        /// Load articles via search
        /// </summary>
        /// <param name="feed"></param>
        private async void AggregateFeed(Feed feed, bool firstLoad = true)
        {
            int indexFeed = _feeds.IndexOf(_feeds.FirstOrDefault(f => f.Id == feed.Id));

            List<Article> articles = new List<Article>();
            //bool isUpdate = _prevSearch == SearchText;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {

                string v = Articles?.First().FullPublishDate.ToUniversalTime().ToString("dd-MM-yyy_HH:mm:ss");
                
                articles = await App.WService.Get<List<Article>>(controller:"feeds", action: feed.IsLoaded ? "update" : null, parameters: feed.IsLoaded ? new string[] { v } : null, jsonBody: $"{{\"search\": \"{feed.Keywords}\"}}", callbackError: (err) =>
                {
                    throw err;
                });

            }
            // Offline search
            else
            {
                var words = feed.Keywords.Split(' ');
                for (int i = 0; i < words.Count(); i++)
                {
                    var word = words[i];
                    articles.AddRange(feed.Articles?.Where((e) => e.Title.Contains(word)));
                }

            }

            if (feed.IsLoaded)
            {
                if (articles.Count > 0)
                    UpdateArticles(articles, feed, indexFeed);
            }
            else
            {
                // Update list of articles
                Articles = new ObservableCollection<Article>(articles);

            }
            SelectedFeed.IsLoaded = true;
            //Feeds[indexFeed] = feed;

            IsRefreshing = false;
            //_isInCustomFeed = true;
            //IsSearchOpen = true;
            //_prevSearch = SearchText;

        }

        // See detail of the article
        public Command GoToDetail
        {
            get
            {
                return new Command(async (id) =>
                {
                    var articlePage = new ArticlePage(Articles.FirstOrDefault(art => art.Id == id.ToString()));


                    await App.Current.MainPage.Navigation.PushAsync(articlePage);
                }); ;
            }
        }

        private bool _isMenuOpen;
        private int _oldIndex;

        public bool IsMenuOpen
        {
            get { return _isMenuOpen; }
            set
            {
                _isMenuOpen = value;
                OnPropertyChanged(nameof(IsMenuOpen));
            }
        }

        public bool IsFromDetail { get; set; }
        public int CurrentFocusIndex { get; set; }
        public bool ListHasBeenUpdated { get; set; }



        /// <summary>
        /// Update the current article feed by adding new elements
        /// </summary>
        /// <param name="articles">new articles</param>
        private void UpdateArticles(List<Article> articles, Feed feed, int indexFeed)
        {
            for (int i = 0; i < articles.Count(); ++i)
            {
                var current = articles[i];
                Article existingArticle = feed.Articles?.FirstOrDefault(a => a.Id == current.Id);
                if (existingArticle == null)
                {

                    Article item = articles.FirstOrDefault(a => a.Id == current.Id);

                    var index = articles.IndexOf(item);

                    // Add article one by one for a better visual effect
                    Articles.Insert(index == -1 ? 0 + i : index, current);
                }
                else
                {
                    int index = Feeds[indexFeed].Articles.IndexOf(existingArticle);
                    // replace the exisiting one with the new one
                    Articles.Remove(existingArticle);
                    Articles.Insert(index, current);
                }


            }
        }
        /// <summary>
        /// Remove a feed from the list
        /// </summary>
        /// <param name="feed">feed we want to remove</param>
        public async void RemoveFeed(Feed feed)
        {


            int feedIndex = Feeds.IndexOf(_feeds.FirstOrDefault(f => f.Id == feed.Id));
            int indexNext = feedIndex + 1;
            int indexPrev = feedIndex - 1;
            //Feeds.Remove(feed);
            Feeds.RemoveAt(feedIndex);
            FeedTabs.RemoveAt(feedIndex);

            Feed feedInView = new Feed();

            await Task.Factory.StartNew(() =>
            {

                if (_selectedFeed.Id == feed.Id)
                {
                    if (_feeds.Count <= 0)
                        return;

                    // We try to establish the next feed
                    if (indexPrev >= 0)
                    {
                        CurrentFeedIndex = indexPrev;
                        feedInView = _feeds[indexPrev];
                    }
                    else
                    {
                        CurrentFeedIndex = indexPrev;
                        feedInView = _feeds[indexNext];
                    }
                }
                else
                    CurrentFeedIndex = 0;

            })
            .ContinueWith((e) =>
            {
                // TODO: Find a way to make sure the next item selected is the previous one (aka feedInView)
                SwitchFeed.Execute(feedInView);

                // Select the first feed
                //SwitchFeed.Execute(_feeds[0]);
            });



            // Switch the the next feed
            //
        }
        /// <summary>
        /// Update a feed
        /// </summary>
        /// <param name="feed">Updated data of feed</param>
        public void UpdateCurrentFeed (Feed feed)
        {
            SelectedFeed = feed;
            Feed feedToUpdate = Feeds.FirstOrDefault((e) => e.Id == feed.Id);

            // Update the item visually
            if (feedToUpdate != null)
            {
                int index = _feeds.IndexOf(feedToUpdate);
                var f =_feeds[index] = feed;

                //CurrentFeedIndex = index;
            }
        }
        /// <summary>
        /// Processed launched when the page reappear
        /// </summary>
        public void Resume()
        {
            CurrentPage.CloseDropdownMenu();
            try
            {

                //CurrentApp.ShowLoadingIndicator();
                foreach (var order in UpdateOrders)
                {
                    int feedIndex = _feeds.IndexOf(order.Feed);

                    if (order.Update == FeedUpdate.Remove)
                    {
                        //RemoveFeed(order.Feed);
                        // Delete the feed
                        App.SqLiteConn.Delete(order.Feed);
                        //SelectTab(feedIndex);
                        FeedTabs.RemoveAt(feedIndex);

                        Refresh(_selectedFeed);

                    }
                    if (order.Update == FeedUpdate.Add)
                    {
                        Feed feed = order.Feed;
                        AddFeed(feed);

                    }
                    if (order.Update == FeedUpdate.Edit)
                    {


                        //Feeds.RemoveAt(feedIndex);
                        //Feeds.Add(order.Feed);

                        ChangeFeed(order.Feed);

                        //CurrentFeedIndex = feedIndex;
                    }
                }
                UpdateOrders.Clear();

                CurrentApp.RemoveLoadingIndicator();
            }
            catch (Exception ex)
            {

            }

           

        }
        /// <summary>
        /// Add a feed to the list: this method not only add it to the list of feed but also create a tab for this feed
        /// </summary>
        /// <param name="feed">feed that will be added</param>
        private void AddFeed(Feed feed)
        {
            Feeds.Add(feed);
            FeedTabs.Add(new()
            {
                Title = feed.Title,

            });
        }

        /// <summary>
        /// Change feed from another feed
        /// </summary>
        /// <param name="feed"></param>
        private void ChangeFeed (Feed feed)
        {
            int feedIndex = _feeds.IndexOf(feed);

            Feeds[feedIndex] = feed;
            //CopyFeedsToTabs();
            FeedTabs[feedIndex].Title = feed.Title;

            Refresh(feed);
        }
        /// <summary>
        /// Copies the items from the <see cref="_feeds"/> collection to the <see cref="FeedTabs"/> collection, transforming them into <see cref="TabButton"/> items.
        /// </summary>
        private void CopyFeedsToTabs()
        {
            FeedTabs.Clear();

            var tabButtons = _feeds.Select(feed => new TabButton
            {
                Id = feed.Id,
                Title = feed.Title
                // Set other properties of TabButton if needed
            });

            foreach (var tabButton in tabButtons)
            {
                FeedTabs.Add(tabButton);
            }
            FeedTabs[_currentFeedIndex].BackgroundColour = (Xamarin.Forms.Color)Application.Current.Resources["PrimaryAccent"];
        }
        /// <summary>
        /// Deselect all the feed tabs
        /// </summary>
        private void DeselectAll()
        {
            foreach (var item in FeedTabs)
            {
                FeedTabs[_currentFeedIndex].BackgroundColour = (Xamarin.Forms.Color)Application.Current.Resources["LightDark"];

            }
        }
        /// <summary>
        /// Select a tab from its index
        /// </summary>
        /// <param name="index">the index of the tab</param>
        private void SelectTab(int index)
        {
            _oldIndex = index;
            FeedTabs[index].BackgroundColour = (Xamarin.Forms.Color)Application.Current.Resources["PrimaryAccent"];
        }
        /// <summary>
        /// Select a tab after the select tab was deleted
        /// </summary>
        /// <param name="index">index of the former tab</param>
        public void SelectDefaultTab(int index)
        {
            int indexNext = index + 1;
            int indexPrev = index - 1;

            int tabIndex = 0;
            if (_feeds.Count <= 0)
                return;

            // We try to establish the next feed
            if (indexPrev >= 0 )
                tabIndex = indexPrev;
            else if (indexNext <= _feeds.Count -1)
                tabIndex = indexPrev;

            // Select the tab that we determined
            SelectTab(tabIndex);

            // Refresh the tab
            Refresh(_feeds[tabIndex]);
        }
        /// <summary>
        /// Delect a tab from its index
        /// </summary>
        /// <param name="index">the index of the tab</param>
        private void DeselectTab(int index)
        {
            try
            {

                FeedTabs[index].BackgroundColour = (Xamarin.Forms.Color)Application.Current.Resources["LightDark"];
            }
            catch 
            {

            }
        }
        /// <summary>
        /// Switch from a tab to another
        /// </summary>
        /// <param name="index"> index of the tab you want to switch to</param>
        private void SwitchTabs(int index)
        {
            // Deselect the previous tab
            //DeselectAll();
            if (_oldIndex <= _feeds.Count -1)
                DeselectTab(_oldIndex);

            // Select the new one
            SelectTab(index);
        }

    }
}
