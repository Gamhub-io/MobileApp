using AresNews.Helpers.Tools;
using AresNews.Models;
using AresNews.Views;
using MvvmHelpers;
using Rg.Plugins.Popup.Extensions;
using SQLiteNetExtensions.Extensions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Command = Xamarin.Forms.Command;

namespace AresNews.ViewModels
{
    public class FeedsViewModel : BaseViewModel
    {
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
        public Xamarin.Forms.Command<Feed> RefreshAll => new Xamarin.Forms.Command<Feed>((feed) =>
        {
            IsRefreshing = true;
            Task.Run(() =>
            {
                this.Refresh(feed);
            });

        });

        public Xamarin.Forms.Command<Feed> SwitchFeed => new Xamarin.Forms.Command<Feed>(async (feed) =>
        {
            if (IsBusy)
                return;
            await Task.Factory.StartNew(() =>
             {

                 // Set new feed
                 CurrentFeed = feed;
                 CurrentFeed.IsLoaded = false;

                 //// Load articles
                 //
             });

            RefreshArticles.Execute(null);
        });
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
            await CurrentPage.Navigation.PushAsync(new EditFeedPage(_currentFeed, this));

        });

        public Xamarin.Forms.Command RefreshArticles => new Xamarin.Forms.Command(async() =>
        {
            if (IsBusy)
                return;
            //IsRefreshing = true;
            await Task.Factory.StartNew(() =>
            {
                try
                {

                    this.Refresh(CurrentFeed);
                }
                catch
                {
                    this.Refresh(_feeds[0]);
                }
            });
        });
		public FeedsViewModel(FeedsPage page)
		{
            CurrentPage = page;
            Feeds = new ObservableCollection<Feed>(App.SqLiteConn.GetAllWithChildren<Feed>());



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


                await Task.Factory.StartNew(() =>
                {

                    AgregateFeed(feed, isFirstLoad);

                    // End the loading indicator
                    IsRefreshing = false;
                    IsBusy = false;

                });
        }
        /// <summary>
        /// Load articles via search
        /// </summary>
        /// <param name="articles"></param>
        private async void AgregateFeed(Feed feed, bool firstLoad = true)
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


        /// <summary>
        /// Update the curent article feed by adding new elements
        /// </summary>
        /// <param name="articles">new articles</param>
        private void UpdateArticles(List<Article> articles, Feed feed, int indexFeed)
        {
            for (int i = 0; i < articles.Count(); ++i)
            {
                var current = articles[i];
                Article existingArticle = feed.Articles.FirstOrDefault(a => a.Id == current.Id);
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


            int indexNext = Feeds.IndexOf(feed) + 1;
            int indexPrev = Feeds.IndexOf(feed) - 1;
            Feeds.Remove(feed);

            Feed feedInView = new Feed();

            await Task.Factory.StartNew(() =>
            {

                if (_currentFeed == feed)
                {
                    if (_feeds.Count <= 0)
                        return;

                    // We try to establish the next feed
                    if (indexPrev >= 0)
                    {
                        feedInView = _feeds[indexPrev];
                    }
                    else
                    {
                        feedInView = _feeds[indexNext];
                    }
                }

            }).ContinueWith((e) =>
            {
                SwitchFeed.Execute(feedInView);
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

                CurrentFeedIndex = index;
            }
        }
        /// <summary>
        /// Processed launched when the page reappear
        /// </summary>
        public void Resume()
        {
            // Get all the feeds regestered
            var curFeeds = new ObservableCollection<Feed>(App.SqLiteConn.GetAllWithChildren<Feed>());

            // We try to figure out if the two feed lists contains the same items
            if (!FeedToolkit.CampareItems(_feeds, curFeeds))
            {
                // Add the last feed added
                Feeds.Add(curFeeds.Last());
            }

        }

    }
}
