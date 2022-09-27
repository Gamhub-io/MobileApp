using AresNews.Models;
using MvvmHelpers;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

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

		public FeedsViewModel()
		{
			Refresh();
        }
		public void Refresh()
		{
            Feeds = new ObservableCollection<Feed>(App.SqLiteConn.GetAllWithChildren<Feed>());

            foreach (var feed in _feeds)
            {
                Task.Run(() => AgregateFeed(feed, false));
            }
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

                string v = feed.Articles?.First().FullPublishDate.ToUniversalTime().ToString("dd-MM-yyy_HH:mm:ss");
                articles = await App.WService.Get<List<Article>>("feeds", firstLoad ? "update" : null, parameters: firstLoad ? new string[] { v } : null, jsonBody: $"{{\"search\": \"{feed.Keywords}\"}}", callbackError: (err) =>
                {

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

            if (firstLoad)
            {
                if (articles.Count > 0)
                    UpdateArticles(articles, feed);
            }
            else
            {
                // Update list of articles
                feed.Articles = new ObservableCollection<Article>(articles);

            }
            Feeds[indexFeed] = feed;
            
            //IsRefreshing = false;
            //_isInCustomFeed = true;
            //IsSearchOpen = true;
            //_prevSearch = SearchText;

        }
        /// <summary>
        /// Update the curent article feed by adding new elements
        /// </summary>
        /// <param name="articles">new articles</param>
        private void UpdateArticles(List<Article> articles, Feed feed)
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
                    feed.Articles.Insert(index == -1 ? 0 + i : index, current);
                }
                else
                {
                    int index = feed.Articles.IndexOf(existingArticle);
                    // replace the exisiting one with the new one
                    feed.Articles.Remove(existingArticle);
                    feed.Articles.Insert(index, current);
                }


            }
        }

    }
}
