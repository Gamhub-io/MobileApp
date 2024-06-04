using AresNews.Helpers.Comparers;
using AresNews.Helpers.Tools;
using AresNews.Models;
using AresNews.Services;
using AresNews.Views;
using MvvmHelpers;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace AresNews.ViewModels
{
    public class NewsViewModel : BaseViewModel
    {

        private bool _isLaunching = true;
        private string _prevSearch;
        private string _lastCallDateTime;

        private int _wifiRestartCount = 0;
        private bool _isInCustomFeed;
        private static object collisionLock = new();
        private bool _isSearching;
        public bool IsSearching
        {
            get
            {
                return _isSearching;
            }
            set
            {
                _isSearching = value;
                OnPropertyChanged(nameof(IsSearching));
            }
        }

        private bool _isSearchOpen;
        public bool IsSearchOpen
        {
            get
            {
                return _isSearchOpen;
            }
            set
            {
                _isSearchOpen = value;
                OnPropertyChanged(nameof(IsSearchOpen));
            }
        }
        private Collection<Feed> Feeds {  get; set; }
        private string _searchText;

        public string SearchText
        {
            get
            {
                return _searchText;
            }
            set
            {
                _searchText = value;

                IsCurrentSearchSaved = Feeds.Any<Feed>(feed => feed.Keywords.ToLower() == value.ToLower());
                OnPropertyChanged(nameof(SearchText));
            }
        }
        private Feed _currentFeed;

        public Feed CurrentFeed
        {
            get { return _currentFeed;; }
            set 
            {
                _currentFeed = value;
                OnPropertyChanged(nameof(CurrentFeed));
            }
        }


        public Command OpenSearch
        {
            get
            {
                return new Command(() =>
                {
                    IsSearching = true;

                });
            }
        }

        private bool _isCurrentSearchSaved;

        public bool IsCurrentSearchSaved
        {
            get { return _isCurrentSearchSaved; }
            set 
            { 
                _isCurrentSearchSaved = value;
                OnPropertyChanged(nameof(IsCurrentSearchSaved));
            }
        }

        private bool _isSearchProcessed;

        public bool IsSearchProcessed
        {
            get { return _isSearchProcessed; }
            set 
            {
                _isSearchProcessed = value;
                OnPropertyChanged(nameof(IsSearchProcessed));
            }
        }

        public Command SaveSearch
        {
            get
            {
                return new Command(() =>
                {
                    if (string.IsNullOrEmpty(SearchText))
                        return;
                    
                    IsCurrentSearchSaved = !_isCurrentSearchSaved;

                    Feed feedTarget = _currentFeed ?? Feeds.FirstOrDefault(fd =>  fd.Title.ToLower() == SearchText.ToLower());

                    if (_isCurrentSearchSaved)
                    {
                        CurrentFeed.Id = Guid.NewGuid().ToString();
                        CurrentFeed.Title = SearchText;
                        CurrentFeed.Keywords = SearchText;
                        CurrentFeed.IsSaved = true;
                        App.SqLiteConn.InsertWithChildren(_currentFeed);
                        Feeds.Add(_currentFeed);

                        // Update the feeds remotely
                        //MessagingCenter.Send<Feed>(_currentFeed, "AddFeed");
                        return;

                    }

                    if (feedTarget?.Keywords == null)
                        feedTarget = Feeds.FirstOrDefault(f => f.Keywords.ToLower() == SearchText.ToLower());

                    App.SqLiteConn.Delete(feedTarget);
                    Feeds.Remove(feedTarget);
                    //MessagingCenter.Send<Feed>(feedTarget, "RemoveFeed");




                });
            }
        }
        public Command CloseSearch
        {
            get
            {
                return new Command(() =>
                {
                    IsSearching = false;

                    if (string.IsNullOrEmpty(_searchText) || !IsSearchProcessed) return;

                    CurrentApp.ShowLoadingIndicator();

                    // Scroll up before fetching the items
                    CurrentPage.ScrollFeed();
                    IsRefreshing = true;
                    _prevSearch = null;

                    // Empty the search bar
                    SearchText = string.Empty;

                    IsSearchProcessed = false;

                });
            }
        }

        // Property list of articles
        private ObservableRangeCollection<Article> _articles;

        public ObservableRangeCollection<Article> Articles
        {
            get { return _articles; }
            set
            {
                _articles = value;
                OnPropertyChanged(nameof(Articles));
                SetProperty(ref _articles, value);
            }
        }
        private ObservableCollection<Article> _unnoticedArticles;

        public ObservableCollection<Article> UnnoticedArticles
        {
            get { return _unnoticedArticles; }
            set
            {
                _unnoticedArticles = value;
                if (_unnoticedArticles?.Count >0)
                    CurrentPage.ShowRefreshButton();
                else
                    CurrentPage.RemoveRefreshButton();

                OnPropertyChanged(nameof(UnnoticedArticles));
            }
        }
        private bool _onTopScroll = true;

        public bool OnTopScroll
        {
            get { return _onTopScroll; }
            set 
            {
                _onTopScroll = value;
                OnPropertyChanged(nameof(OnTopScroll));
            }
        }


        public App CurrentApp { get; }
        private NewsPage CurrentPage { get; set; }
        // Command to add a Bookmark
        private readonly Command _addBookmark;

        public Command AddBookmark
        {
            get
            {
                return _addBookmark;
            }
        }
        // Command to refresh the news feed
        private readonly Command _refreshFeed;

        public Command RefreshFeed
        {
            get { return _refreshFeed; }
        }

        public Command LoadSearch
        {
            get; private set;
        }
        // Command to add a Bookmark
        private readonly Command _shareArticle;

        public Command ShareArticle
        {
            get { return _shareArticle; }
        }
        // See detail of the article
        public Command GoToDetail
        {
            get
            {
                return new Command( (id) =>
                {
                    var articlePage = new ArticlePage(_articles.FirstOrDefault(art => art.Id == id.ToString()));


                    _= App.Current.MainPage.Navigation.PushAsync(articlePage);
                }); ;
            }
        }
        public Command UncoverNewArticles { get; private set; }

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

        public bool IsFirstLoad { get; private set; } = true;
        public bool IsSearchLoading { get; private set; }

        public NewsViewModel(NewsPage currentPage)
        {
            CurrentApp = App.Current as App;
            CurrentPage = currentPage;
            Feeds = new Collection<Feed>(App.SqLiteConn.GetAllWithChildren<Feed>());
            UnnoticedArticles = new();
            Articles = new ObservableRangeCollection<Article>(GetBackupFromDb().OrderBy(a => a.Time).ToList());
            UncoverNewArticles = new Command(() =>
            {
                if (UnnoticedArticles == null)
                    return;
                if (UnnoticedArticles.Count <= 0)
                    return;

                CurrentApp.ShowLoadingIndicator();
                _ = Task.Run(async () =>
                {
                    // Scroll up
                    CurrentPage.ScrollFeed();

                    // Add the unnoticed articles
                    UpdateArticles(UnnoticedArticles);

                    UnnoticedArticles.Clear();

                }).ContinueWith(res => CurrentApp.RemoveLoadingIndicator());

            });

            CurrentFeed = new Feed();
            _isLaunching = true;


            // Handle if a article change sees a change of bookmark state
            MessagingCenter.Subscribe<Article>(this, "SwitchBookmark", (sender) =>
            {
                var page = ((IShellSectionController)Shell.Current?.CurrentItem?.CurrentItem).PresentedPage;

                // Escape is the current page is the news page
                if (page is NewsPage)
                    return;

                // Select the article
                Article article = _articles.FirstOrDefault(a => a.Id == sender.Id);

                // end there if the article is not listed anymore
                if (article is null)
                    return;

                // Get article index
                int index = _articles.IndexOf(article);

                try
                {
                    if (_articles.Count > 0)
                    {
                        // Remove the previous one 
                        Articles.Remove(article);

                        article.IsSaved = !article.IsSaved;

                        // to add the new one
                        Articles.Insert(index, article);
                    }


                }
                catch (Exception ex)
                {
#if DEBUG
                    throw ex;

#endif
                }
            });

            _addBookmark = new Command((id) =>
            {
                var article = new Article();
                // Get the article
                article = _articles.FirstOrDefault(art => art.Id == id.ToString());



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

            _refreshFeed = new Command<bool>( (isAll) =>
               {
                   
                   if (IsSearchOpen)
                   {
                       if (string.IsNullOrEmpty(SearchText))
                           return;
                       // Fetch the article
                       _ = FetchArticles(true);
                       return;
                   }
                   // Fetch the article
                   _ = FetchArticles();
               });
            LoadSearch = new Command(async () =>
            {
                if (IsSearchLoading)
                    return;
                CurrentApp.ShowLoadingIndicator();
                IsSearchProcessed = true;
                IsSearchLoading = true;

                await FetchArticles().ContinueWith((res) =>
                {
                    CurrentApp.RemoveLoadingIndicator();
                    IsSearchLoading = false;
                });
            });
            // Set command to share an article
            _shareArticle = new Command((id) =>
            {
                // Get selected article
                var article = _articles.FirstOrDefault(art => art.Id == id.ToString());

                _= Share.RequestAsync(new ShareTextRequest
                {
                    Uri = article.Url,
                    Title = "Share this article",
                    Subject = article.Title,
                    Text = article.Title
                });
            });
        }

        /// <summary>
        /// Fetch all the articles
        /// </summary>
        public async Task FetchArticles(bool isFullRefresh = false)
        {

            var articles = new ObservableRangeCollection<Article>();
            if (isFullRefresh)
            {
                CurrentApp.ShowLoadingIndicator();
                // the articles of the last 2 months
                articles = new (await CurrentApp.DataFetcher.GetMainFeedUpdate().ConfigureAwait(false));
                _isLaunching = false;
                
                // Reload the feed
                Articles.Clear();
                Articles = new ObservableRangeCollection<Article>(articles.Where(article => article.Blocked == null || article.Blocked == false));


                _ = RefreshDB();
                // Register date of the refresh
                IsRefreshing = false;
                IsSearchOpen = false;
                _prevSearch = string.Empty;
                CurrentApp.RemoveLoadingIndicator();
                return;
            }

            try
            {
                if (_articles?.Count() > 0)
                    _lastCallDateTime = _articles?.First().FullPublishDate.ToUniversalTime().ToString("dd-MM-yyy_HH:mm:ss");
                
                // If we want to fetch the articles via search
                if (!string.IsNullOrEmpty(SearchText) && IsSearching == true )
                {
                    await SearchArticles(articles);
                    return;
                }
                if (string.IsNullOrEmpty(_lastCallDateTime))
                {

                    Articles = new ObservableRangeCollection<Article>((await CurrentApp.DataFetcher.GetMainFeedUpdate().ConfigureAwait(false)).Where(article => article.Blocked == null || article.Blocked == false));
                    
                    IsRefreshing = false;
                    _isLaunching = false;
                    _= RefreshDB();
                    return;
                }
               if (_articles?.Count() > 0)
               {
                   
                       articles = new ObservableRangeCollection<Article>((await CurrentApp.DataFetcher.GetMainFeedUpdate(_lastCallDateTime).ConfigureAwait(false)).Where(article => article.Blocked == null || article.Blocked == false));
               }
               else
               {
                   if (_isLaunching)
                   {
                       try
                       {
                           Articles = new ObservableRangeCollection<Article>((await App.WService.Get<ObservableCollection<Article>>("feeds", jsonBody: null)).Where(article => article.Blocked == null || article.Blocked == false));

                           // Manage backuo
                           _ = RefreshDB();

                       }
                       catch
                       {
                       }

                       _isLaunching = false;
                       IsRefreshing = false;
                       return;
                   }
                  articles = new ObservableRangeCollection<Article>((await App.WService.Get<ObservableRangeCollection<Article>>("feeds", jsonBody: null)).Where(article => article.Blocked == null || article.Blocked == false));

               }
            }
            catch (Exception ex)
            {
                // BLAME: the following lines are disgusting but it works 
                // TODO: change this if possible
                if (ex.Message.Contains("Network subsystem is down") && Device.RuntimePlatform == Device.Android && _wifiRestartCount < 3)
                {
                    // Restart wifi: only works with android < Q
                    if (DependencyService.Get<IInternetManagement>().TurnWifi(false))
                    {
                        _ = DependencyService.Get<IInternetManagement>().TurnWifi(true);

                        _wifiRestartCount++;

                        // call the thing again
                        _ = FetchArticles();
                        return;

                    }
                }
                //articles = new ObservableCollection<Article> (GetBackupFromDb().OrderBy(a => a.Time).ToList()) ;

                var page = (NewsPage)((IShellSectionController)Shell.Current?.CurrentItem?.CurrentItem).PresentedPage;
                page.DisplayOfflineMessage(ex.Message);
            }

            // To avoid crashes: if this number is out of range we end the process
            if (articles == null || articles.Count() <= 0)
            {
                IsRefreshing = false;
                return;
            }

            //MainThread.BeginInvokeOnMainThread(() =>
            //{
                
                    if (OnTopScroll)
                    {
                        // Update list of articles
                        UpdateArticles(articles);
                        try
                        {
                            // Manage backup
                            _ = RefreshDB();

                        }
                        catch
                        {
                        }
                        finally
                        {
                            _isLaunching = false;
                        }

                        IsRefreshing = false;
                    }
                        
                    else
                    {
                        UnnoticedArticles = new ObservableCollection<Article>(articles);
                    }
            //});


        }
        /// <summary>
        /// Update the current article feed by adding new elements
        /// </summary>
        /// <param name="articles">new articles</param>
        private void UpdateArticles(IEnumerable<Article> articles)
        {
            // Create a copy of the input ObservableCollection
            Collection<Article> listArticle = new (articles.ToList());

            // Lists to store articles to be added and updated
            Collection<Article> articlesToUpdate = new ();

            // Iterate through the copied list of articles
            foreach (var current in listArticle)
            {
                // Check if the current article already exists in the _articles collection
                Article existingArticle = _articles.FirstOrDefault(a => a.MongooseId == current.MongooseId);

                if (existingArticle == null)
                    // Article doesn't exist in _articles, add it to the articlesToAdd list
                    Articles.Insert(0,current);
                else if (!existingArticle.IsEqualTo(current))
                    // Article exists in _articles, add it to the articlesToUpdate list
                    articlesToUpdate.Add(current);
            }
            foreach (var articleToUpdate in articlesToUpdate)
            {
                // Find the existing article to be updated in the '_articles' collection
                Article existingArticle = _articles.FirstOrDefault(a => a.Id == articleToUpdate.Id);
                if (existingArticle != null)
                {
                    // Get the index of the existing article in the 'Articles' collection
                    int index = _articles.IndexOf(existingArticle);

                    // Remove the existing article and insert the updated one at the same index
                    Articles.Remove(existingArticle);
                    Articles.Insert(index, articleToUpdate);
                }
                // If existingArticle is null, handle the case where the article to update was not found
            }
        }
        /// <summary>
        /// Sync the local db
        /// </summary>
        /// <returns></returns>
        private Task RefreshDB()
        {
            try
            {
                return Task.Run(() =>
                {
                    lock (collisionLock)
                    {
                        using var conn = new SQLiteConnection(App.BackUpConn.DatabasePath);

                        conn.DeleteAll<Article>();

                        conn.InsertAllWithChildren(_articles);

                        foreach (var source in _articles.Select(a => a.Source).Distinct().ToList())
                        {
                            conn.InsertOrReplace(source);

                        }
                    }
                });
            }
            finally
            {
                //App.BackUpConn.Close();
            };
        }

        /// <summary>
        /// Load articles via search
        /// </summary>
        /// <param name="articles"></param>
        private async Task SearchArticles(ObservableRangeCollection<Article> articles)
        {
            bool isUpdate = _prevSearch == SearchText;
            string timeParam = string.Empty;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                if (isUpdate)
                    timeParam = _articles?.First().FullPublishDate.ToUniversalTime().ToString("dd-MM-yyy_HH:mm:ss");
                articles = await App.WService.Get<ObservableRangeCollection<Article>>(controller:"feeds", action: isUpdate ? "update": null, parameters: isUpdate?  new string[] { timeParam } : null, jsonBody: $"{{\"search\": \"{SearchText}\"}}").ConfigureAwait(false);
                
            }
            // Offline search
            else
            {
                var words = SearchText.Split(' ');
                for (int i = 0; i < words.Count(); i++)
                {
                    var word = words[i];
                    articles.AddRange(_articles.Where((e) => e.Title.Contains(word)));
                }
                
            }

            if (isUpdate)
            {
                if (articles.Count > 0)
                    UpdateArticles(articles.Where(article => article.Blocked == null || article.Blocked == false));
            }
            else
            {
                // Update list of articles
                Articles = new ObservableRangeCollection<Article>(articles.Where(article => article.Blocked == null || article.Blocked == false));

            }

            IsRefreshing = false;
            _isInCustomFeed = true;
            IsSearchOpen = true;
            _prevSearch = SearchText;

        }
        /// <summary>
        /// Get all the articles from the db
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Article> GetBackupFromDb()
        {
            return App.BackUpConn.GetAllWithChildren<Article>(recursive: true).Where(article => article.Blocked == null || article.Blocked == false).Reverse<Article>();
        }
        void ObservableCollectionCallback(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            // `lock` ensures that only one thread access the collection at a time
            lock (collection)
            {
                accessMethod?.Invoke();
            }
        }/// <summary>
         /// Processed launched when the page reappear
         /// </summary>
        public void Resume()
        {
            if (IsFirstLoad)
            {
                CurrentApp.ShowLoadingIndicator();
                _ = FetchArticles(_articles?.Count <= 0).ContinueWith(res =>
                  CurrentApp.RemoveLoadingIndicator());

                IsFirstLoad = false;

            }
            // Get all the feeds regestered
            var curFeeds = new ObservableCollection<Feed>(App.SqLiteConn.GetAllWithChildren<Feed>());

            // We try to figure out if the two feed lists contains the same items
            if (!FeedToolkit.CampareItems(Feeds, curFeeds))
            {
                // Reload the feeds
                Feeds = curFeeds;
            }

        }

    }
}