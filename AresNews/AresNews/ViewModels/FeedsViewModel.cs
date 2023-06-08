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
        private int _currentFeedIndex;

        public int CurrentFeedIndex
        {
            get { return _currentFeedIndex; }
            set 
            {
                _currentFeedIndex = value;
                OnPropertyChanged(nameof(CurrentFeedIndex));
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
        private Feed _currentFeed;

        public Feed CurrentFeed
        {
            get { return _currentFeed; }
            set 
            {
                _currentFeed = value;
                //CurrentFeedIndex = _feeds.IndexOf(value);
                OnPropertyChanged(nameof(CurrentFeed));
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
                 CurrentFeed = feed;
                 CurrentFeed.IsLoaded = false;

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
            await CurrentPage.Navigation.PushPopupAsync(new DeleteFeedPopUp(_currentFeed, this));

        });

        public Xamarin.Forms.Command<Feed> Rename => new Xamarin.Forms.Command<Feed>(async (feed) =>
        {
            await CurrentPage.Navigation.PushPopupAsync(new RenameFeedPopUp(_currentFeed, this));

        });

        public Xamarin.Forms.Command<Feed> Edit => new Xamarin.Forms.Command<Feed>(async (feed) =>
        {
            IsFromDetail = true;
            CurrentFocusIndex = _feeds.IndexOf(_currentFeed);
            await CurrentPage.Navigation.PushAsync(new EditFeedPage(_currentFeed, this));

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

                        if (CurrentFeed == null)
                        {
                            CurrentFeed = _feeds[0];

                        }
                        this.Refresh(CurrentFeed);
                    }
                    catch
                    {
                        this.Refresh(_feeds[0]);
                    }
                });
            });
        });
		public FeedsViewModel(FeedsPage page)
        {
            CurrentApp = App.Current as App;
            CurrentPage = page;
            Feeds = new ObservableCollection<Feed>(App.SqLiteConn.GetAllWithChildren<Feed>());
            //CurrentPage.ResetTabs();

            IsRefreshing = true;

            RefreshArticles.Execute(null);

            UpdateOrders = new List<UpdateOrder>();

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
                //Feed item = _feeds.First(f => f.Id == sender.Id);
                ////if (_feeds.Count > 1 && _currentFeed == item )
                ////    CurrentFeed = _feeds[0];
                ////Feeds.Remove(item);
                //Delete.Execute(item);

                if (sender == null)
                    return;

                UpdateOrders.Add(new UpdateOrder
                {
                    Update = UpdateOrder.FeedUpdate.Remove,
                    Feed = sender
                });
                // Remove the item from the separate collection
                //Feed feedToRemove = _feeds.FirstOrDefault(f => f.Keywords.ToLower() == sender.Keywords.ToLower());
                //int index = _feeds.IndexOf(feedToRemove);

                ////CurrentPage.RemoveTab(_feeds.IndexOf(feedToRemove));

                //if (feedToRemove != null)
                //{
                //    CurrentPage.ResetTabs();
                //    Feeds.Remove(feedToRemove);
                //    CurrentPage.RemoveTab(index);
                //    //var newFeeds = new ObservableCollection<Feed>(_feeds);
                //    // Update the ItemsSource of the CarouselView with the new collection
                //    //Feeds.Clear();
                //    //Feeds = newFeeds;
                //}


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
                bool isFirstLoad = feed != _currentFeed;
                if (isFirstLoad)
                {

                    CurrentFeed = feed;
                    CurrentFeed.IsLoaded = false;
                }


                await Task.Run(() =>
                {

                    AggregateFeed(feed, isFirstLoad);

                    // End the loading indicator
                    IsRefreshing = false;
                    IsBusy = false;

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
            CurrentFeed.IsLoaded = true;
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

            Feed feedInView = new Feed();

            await Task.Factory.StartNew(() =>
            {

                if (_currentFeed.Id == feed.Id)
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
                //SwitchFeed.Execute(feedInView);

                // Select the first feed
                SwitchFeed.Execute(_feeds[0]);
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
            CurrentFeed = feed;
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
            //CurrentPage.ResetTabs();
            //Task.Run(() =>
            //{

                //Thread.Sleep(10000);
                foreach (var order in UpdateOrders)
                {
                    if (order.Update == FeedUpdate.Remove)
                    {
                        //RemoveFeed(order.Feed);
                        // Delete the feed
                        App.SqLiteConn.Delete(order.Feed);

                    }
                    if (order.Update == FeedUpdate.Add)
                    {
                        Feeds.Add(order.Feed);

                    }
                }
                UpdateOrders.Clear();
            //});
            // Get all the feeds registered
            //var curFeeds = new ObservableCollection<Feed>(App.SqLiteConn.GetAllWithChildren<Feed>());

            //CurrentPage.ResetTabs();
            //Feeds = new ObservableCollection<Feed>(curFeeds);
            // We try to figure out if the two feed lists contains the same items
            //if (!_feeds.SequenceEqual(curFeeds))
            //{
            //    // Add the last feed added
            //    Feeds = new ObservableCollection<Feed>(curFeeds);
            //    CurrentPage.ResetTabs();
            //    //Feeds.Add(curFeeds.Last());
            //}

        }
        

    }
}
