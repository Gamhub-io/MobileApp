using AresNews.Models;
using AresNews.Views;
using MvvmHelpers;
using MvvmHelpers.Commands;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
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
                OnPropertyChanged(nameof(CurrentFeed));
            }
        }

        private Xamarin.Forms.Command<Feed> _refreshAll ;
        public Xamarin.Forms.Command<Feed> RefreshAll => _refreshAll;

        private Xamarin.Forms.Command<Feed> _refreshArticles;
        public Xamarin.Forms.Command<Feed> RefreshArticles =>  _refreshArticles;
		public FeedsViewModel()
		{
            Feeds = new ObservableCollection<Feed>(App.SqLiteConn.GetAllWithChildren<Feed>());

            _refreshAll = new Xamarin.Forms.Command<Feed>((feed) =>
            {
                this.Refresh(feed);
                //CurrentFeed = feed;

                IsRefreshing = true;

            });

            _refreshArticles = new Xamarin.Forms.Command<Feed>((feed) =>
            {
                
                this.Refresh(feed);
            });


            // Set command to share an article
            _shareArticle = new Command(async (id) =>
            {
                // Get selected article
                var article = CurrentFeed.Articles.FirstOrDefault(art => art.Id == id.ToString());

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
                article = CurrentFeed.Articles.FirstOrDefault(art => art.Id == id.ToString());



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
        public void Refresh(Feed feed)
		{
            bool isFirstLoad = feed != _currentFeed;
            if (isFirstLoad) 
            {

                CurrentFeed = feed;
                CurrentFeed.IsLoaded = false;
            }


            Task.Factory.StartNew(() =>
            {

                AgregateFeed(feed, isFirstLoad);
            });
            //foreach (var feed in _feeds)
            //{
            //    Task.Run(() => AgregateFeed(feed, false));
            //}
        }
        /// <summary>
        /// Load articles via search
        /// </summary>
        /// <param name="articles"></param>
        private async void AgregateFeed(Feed feed, bool firstLoad = true)
        {
            int indexFeed = _feeds.IndexOf(feed);
            List<Article> articles = new List<Article>();
            //bool isUpdate = _prevSearch == SearchText;

            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {

                string v = Articles?.First().FullPublishDate.ToUniversalTime().ToString("dd-MM-yyy_HH:mm:ss");
                var f = await App.WService.Get("feeds", feed.IsLoaded ? "update" : null, parameters: feed.IsLoaded ? new string[] { v } : null, jsonBody: $"{{\"search\": \"{feed.Keywords}\"}}", callbackError: (err) =>
                {

                });
                articles = await App.WService.Get<List<Article>>("feeds", feed.IsLoaded ? "update" : null, parameters: feed.IsLoaded ? new string[] { v } : null, jsonBody: $"{{\"search\": \"{feed.Keywords}\"}}", callbackError: (err) =>
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

        // See detail of the art;[p;l;;l.icle
        public Command GoToDetail
        {
            get
            {
                return new Command(async (id) =>
                {
                    var articlePage = new ArticlePage(_currentFeed.Articles.FirstOrDefault(art => art.Id == id.ToString()));


                    await App.Current.MainPage.Navigation.PushAsync(articlePage);
                }); ;
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

    }
}
