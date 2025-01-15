
using GamHubApp.Models;
using GamHubApp.Views;
using Newtonsoft.Json;
using SQLiteNetExtensions.Extensions;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Command = Microsoft.Maui.Controls.Command;
using MvvmHelpers;

namespace GamHubApp.ViewModels
{
    public class FeedsViewModel : BaseViewModel
    {
        private bool _dataLoaded = false;
        public List<UpdateOrder> UpdateOrders { get; private set; }
        private ObservableCollection<Feed> _feeds = new();

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
                //SwitchTabs(_currentFeedIndex);

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

		private ObservableRangeCollection<Article> _articles;

		public ObservableRangeCollection<Article> Articles
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

        private TabButton _selectedFeedTab;
        public TabButton SelectedFeedTab
        {
            get { return _selectedFeedTab; }
            set
            {
                if (_selectedFeedTab != value)
                {
                    // Set the IsSelected property for the newly selected tab
                    _selectedFeedTab = value;
                    int index = _feedTabs.IndexOf(value);
                    FeedTabs[index].IsSelected = true;
                    SelectedFeed = _feeds[index];
                    // Reset the IsSelected property for other tabs
                    for (int i = 0; i < _feedTabs.Count; i++)
                    {
                        if (i != index)
                        {
                            FeedTabs[i].IsSelected = false;
                        }
                    }

                    OnPropertyChanged(nameof(SelectedFeedTab));
                }
            }
        }

        public Command<Feed> RefreshAll => new Command<Feed>((feed) =>
        {
            IsRefreshing = true;
            Refresh(feed);

        });

        public Command<Feed> SwitchFeed => new Command<Feed>((feed) =>
        {
            if (IsBusy)
                return;
            //IsRefreshing = true;

            CurrentApp.ShowLoadingIndicator();
            _ = SwitchFeedAsync(feed);

            RefreshArticles.Execute(null);
            CurrentApp.RemoveLoadingIndicator();

        });

        private async Task SwitchFeedAsync(Feed feed)
        {
            await Task.Run(() =>
            {
                UnnoticedArticles.Clear();
                // Set new feed
                SwitchTabs(_feeds.IndexOf(feed));
                SelectedFeed.IsLoaded = false;
            });
        }

        public App CurrentApp { get; }

        public Command UncoverNewArticles { get; private set; }
        private FeedsPage CurrentPage { get; set; }

        public Command<Feed> Delete => new Command<Feed>( (feed) =>
        {
            CurrentApp.OpenPopUp (new DeleteFeedPopUp(_selectedFeed, this), CurrentPage);

        });

        public Command<Feed> Rename => new Command<Feed>( (feed) =>
        {
            CurrentApp.OpenPopUp (new RenameFeedPopUp(_selectedFeed, this), CurrentPage);

        });

        public Command<Feed> Edit => new Command<Feed>(async (feed) =>
        {
            IsFromDetail = true;
            CurrentFocusIndex = _feeds.IndexOf(_selectedFeed);
            await CurrentPage.Navigation.PushAsync(new EditFeedPage(_selectedFeed, this));

        });

        private bool _onTopScroll;

        public bool OnTopScroll
        {
            get { return _onTopScroll; }
            set
            {
                _onTopScroll = value;
                OnPropertyChanged(nameof(OnTopScroll));
            }
        }

        public Microsoft.Maui.Controls.Command RefreshArticles => new Microsoft.Maui.Controls.Command(() =>
        {


            if (IsBusy)
                return;
            MainThread.BeginInvokeOnMainThread(async () =>
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
                        Refresh(SelectedFeed);
                    }
                    catch
                    {
                        // in case of error, refresh the first feed
                        RefreshFirstFeed();
                    }
                });
            });
            _dataLoaded = true;
        });

		public FeedsViewModel(FeedsPage page)
        {
            // CurrentApp and CurrentPage will allow use to access to global properties
            CurrentApp = App.Current as App;
            CurrentPage = page;
            
            // Instantiate definitions 
            FeedTabs = new ObservableRangeCollection<TabButton>();
            Feeds = new ObservableCollection<Feed>(App.SqLiteConn.GetAllWithChildren<Feed>());
            _articles = new ObservableRangeCollection<Article>();

            // Organise feeds into tabs
            CopyFeedsToTabs();

            // Select first item
            CurrentFeedIndex = 0;
            UncoverNewArticles = new Command(() =>
            {
                if (UnnoticedArticles == null)
                    return;
                if (UnnoticedArticles.Count <= 0)
                    return;

                CurrentApp.ShowLoadingIndicator();
                int indexFeed = _feeds.IndexOf(_feeds.FirstOrDefault(f => f.Id == SelectedFeed.Id));
                _ = Task.Run(() =>
                {
                    // Scroll up
                    CurrentPage.ScrollFeed();

                    // Add the unnoticed articles
                    UpdateArticles(UnnoticedArticles.ToList(), SelectedFeed, indexFeed);

                    UnnoticedArticles.Clear();

                }).ContinueWith(res => CurrentApp.RemoveLoadingIndicator());

            });


            UpdateOrders = new List<UpdateOrder>();


            if (_feeds.Count <= 0)
                return;

            IsRefreshing = true;

            RefreshArticles.Execute(null);


            MessagingCenter.Subscribe<Feed>(this, "AddFeed", (sender) =>
            {;

                AddFeed(sender);

            });

            MessagingCenter.Subscribe<Feed>(this, "RemoveFeed", (sender) =>
            {

                if (sender == null)
                    return;
                _ = RemoveFeed(sender);


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

                _ = Share.RequestAsync(new ShareTextRequest
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

                // Say the the bookmark has been updated
                MessagingCenter.Send<Article>(article, "SwitchBookmark");


            });
        }

        private readonly Command _shareArticle;

        public Command ShareArticle
        {
            get { return _shareArticle; }
        }

        private ObservableCollection<Article> _unnoticedArticles;

        public ObservableCollection<Article> UnnoticedArticles
        {
            get { return _unnoticedArticles; }
            set
            {
                _unnoticedArticles = value;
                if (_unnoticedArticles?.Count > 0)
                    CurrentPage.ShowRefreshButton();
                else
                    CurrentPage.RemoveRefreshButton();

                OnPropertyChanged(nameof(UnnoticedArticles));
            }
        }

        public Command<string> FeedSelect
        {
            get { return new Command<string> ((feedId) =>
            {
                if (string.IsNullOrEmpty(feedId))
                    return;

                //Feed nextFeed = Feeds.FirstOrDefault(feed => feed.Id == feedId);
                int nextIndex = _feeds.IndexOf(_feeds.FirstOrDefault(feed => feed.Id == feedId));

                if (nextIndex == -1)
                    return;
                SelectedFeedTab = _feedTabs[nextIndex]; 


                // Flag all the other feeds as unloaded
                for (int i = 0; i < _feeds.Count; i++)
                {
                    if (i != nextIndex)
                    {
                        _feeds[i].IsLoaded = false;

                    }
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
        public void Refresh(Feed feed)
		{
            if (IsBusy)
                return;

            IsBusy = true;
            CurrentApp.ShowLoadingIndicator();

            // Determine wether or not it's the first time loading the article of this feed
            bool isFirstLoad = _articles == null || _articles.Count <= 0;



            _ = AggregateFeed(feed, isFirstLoad).ContinueWith(res =>
            {
                // End the loading indicator
                IsRefreshing = false;
                IsBusy = false;
                CurrentApp.RemoveLoadingIndicator();
            });
        }

        /// <summary>
        /// Load articles via search
        /// </summary>
        /// <param name="feed">feed we are searching</param>
        private async Task AggregateFeed(Feed feed, bool firstLoad = true)
        {
            int indexFeed = _feeds.IndexOf(_feeds.FirstOrDefault(f => f.Id == feed.Id));

            List<Article> articles = new List<Article>();
            
            // Figure out if the feed deserve an update
            string timeUpdate = string.Empty;

            if (Articles?.Count != 0)
                timeUpdate = Articles?.First().FullPublishDate.ToUniversalTime().ToString("dd-MM-yyy_HH:mm:ss");

            bool needUpdate = feed.IsLoaded && !string.IsNullOrEmpty(timeUpdate);

            // Make sure we have internet connection
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                articles = (await CurrentApp.DataFetcher.GetFeedArticles(feed.Keywords, 
                                                                         timeUpdate, 
                                                                         needUpdate)).ToList();

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

            if (needUpdate)
            {
                if (articles.Count > 0)
                    if (OnTopScroll)
                        UpdateArticles(articles, feed, indexFeed);
                    else
                        UnnoticedArticles = new ObservableCollection<Article>( articles);
            }
            else
                // Update list of articles
                InsertArticles(articles);
            SelectedFeed.IsLoaded = true;

            IsRefreshing = false;

        }

        /// <summary>
        /// Insert a set of articles in the current list
        /// </summary>
        /// <param name="articles">articles to add</param>
        private void InsertArticles(IEnumerable<Article> articles)
        {
            ObservableRangeCollection<Article> articlesOld = new (_articles);
            Articles = new ObservableRangeCollection<Article>();

            Articles.AddRange(articles);
            if (articlesOld.Any())
                Articles.AddRange(articlesOld);
        }

        // See detail of the article
        public Command GoToDetail
        {
            get
            {
                return new Command( (id) =>
                {
                    var articlePage = new ArticlePage(Articles.FirstOrDefault(art => art.Id == id.ToString()));


                    _ = App.Current.MainPage.Navigation.PushAsync(articlePage);
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
        public async Task RemoveFeed(Feed feed)
        {
            CurrentApp.ShowLoadingIndicator();
            IsBusy = true;
            int feedIndex = Feeds.IndexOf(_feeds.FirstOrDefault(f => f.Id == feed.Id));
            int indexNext = feedIndex + 1;
            
            int indexPrev = 0;
            if (feedIndex > 0)
                indexPrev = feedIndex - 1;
            //Feeds.Remove(feed);
            Feeds.RemoveAt(feedIndex);
            FeedTabs.RemoveAt(feedIndex);

            Feed feedInView = new Feed();

            await Task.Factory.StartNew(() =>
            {
                if (_selectedFeed == null)
                    CurrentFeedIndex = 0;
                if (_selectedFeed.Id == feed.Id)
                {
                    if (_feeds.Count <= 0)
                    {
                        Articles?.Clear();
                        return;

                    }

                    // We try to establish the next feed
                    if (indexPrev >= 0)
                    {
                        CurrentFeedIndex = indexPrev;
                        feedInView = _feeds[indexPrev];
                    }
                    else
                    {
                        CurrentFeedIndex = indexNext;
                        feedInView = _feeds[indexNext];
                    }
                }
                else
                    CurrentFeedIndex = 0;

            })
            .ContinueWith((Action<Task>)((e) =>
            {
                // Load selected feed
                _ = SwitchFeedAsync(feedInView).ContinueWith((res) =>
                {

                    CurrentApp.RemoveLoadingIndicator();

                });
            }));
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
            }
        }

        /// <summary>
        /// Processed launched when the page reappear
        /// </summary>
        public void Resume()
        {
            if (!_dataLoaded)
            {
                Feeds = new ObservableCollection<Feed>(App.SqLiteConn.GetAllWithChildren<Feed>());

                // Refresh the first feed
                RefreshFirstFeed();

                // Organise feeds into tabs
                CopyFeedsToTabs();

                return;
            };

            CurrentPage.CloseDropdownMenu();
            try
            {
                UpdateFeeds();

                if (_selectedFeed != null)
                    Refresh(_selectedFeed);
            }
            catch (Exception ex)
            {
#if DEBUG
                Debug.WriteLine($"Selected Feed {JsonConvert.SerializeObject(_selectedFeed)}");
                throw ex;
#endif
            }
        }

        /// <summary>
        /// Method to refresh the first feed
        /// </summary>
        private void RefreshFirstFeed()
        {
            if (_feeds.Count > 0)
                SelectedFeed = _feeds[0];
        }

        /// <summary>
        /// Update the feeds list
        /// </summary>
        private void UpdateFeeds()
        {
            if (!_dataLoaded) return;
            if (_feeds == null /*|| _selectedFeed ==null*/)
                return;

            // Get the updated list of feed
            Collection<Feed> updatedFeeds = new ObservableCollection<Feed>(App.SqLiteConn.GetAllWithChildren<Feed>());

            List<Feed> newFeeds = updatedFeeds.Where(feed => !_feeds.Any(item => item.Id == feed.Id)).ToList();
            List<Feed> removedFeeds = _feeds.Where(feed => !updatedFeeds.Any(item => item.Id == feed.Id)).ToList();

            // Add the new feeds
            foreach (var feed in newFeeds)
            {
                AddFeed(feed);
            }
            // Remove the outdated feeds
            foreach (var feed in removedFeeds)
            {
                _ =RemoveFeed(feed);
            }
        }

        /// <summary>
        /// Add a feed to the list: this method not only add it to the list of feed but also create a tab for this feed
        /// </summary>
        /// <param name="feed">feed that will be added</param>
        private void AddFeed(Feed feed)
        {
            Feeds.Add(feed);

            // Add a tab for this feed
            FeedTabs.Add(new()
            {
                Id = feed.Id,
                Title = feed.Title,

            });
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

            if (FeedTabs.Count >0)
                FeedTabs[_currentFeedIndex].IsSelected = true;

        }

        /// <summary>
        /// Select a tab from its index
        /// </summary>
        /// <param name="index">the index of the tab</param>
        private void SelectTab(int index)
        {
            _oldIndex = index;


            FeedTabs[index].IsSelected = true;
        }

        /// <summary>
        /// Select a tab after the select tab was deleted
        /// </summary>
        /// <param name="fromIndex">index of the former tab</param>
        public void SelectDefaultTab(int fromIndex)
        {
            int indexNext = fromIndex + 1;
            int indexPrev = fromIndex - 1;

            int tabIndex = 0;
            if (_feeds.Count <= 0)
                return;

            // We try to establish the next feed
            if (indexPrev >= 0 )
                tabIndex = indexPrev;
            else if (indexNext <= _feeds.Count -1)
                tabIndex = indexPrev;

            if (tabIndex == -1)
                tabIndex ++;
            // Select the tab that we determined
            SelectTab(tabIndex);

            // Refresh the tab
            //Refresh(_feeds[tabIndex]);
            SelectedFeed = _feeds[tabIndex];
        }

        /// <summary>
        /// Remove a feed using it's index
        /// </summary>
        /// <param name="feedIndex">index of the feed</param>
        public void RemoveFeedByIndex(int feedIndex)
        {
            Articles?.Clear();
            Feeds.RemoveAt(feedIndex);
            SelectDefaultTab(feedIndex);
        }

        /// <summary>
        /// Delect a tab from its index
        /// </summary>
        /// <param name="index">the index of the tab</param>
        private void DeselectTab(int index)
        {
            try
            {
                if (index == -1)
                    index = 0;
                //FeedTabs[index].BackgroundColour = (Xamarin.Forms.Color)Application.Current.Resources["LightDark"];
                if (FeedTabs.Count <= 0)
                    return;
                FeedTabs[index].IsSelected = false;
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
            if (index == -1)
                return;


            // Deselect the previous tab
            if (_oldIndex <= _feeds.Count -1)
                DeselectTab(_oldIndex);

            // Select the new one
            SelectTab(index);
        }

    }
}
