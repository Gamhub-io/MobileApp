using AresNews.Models;
using AresNews.Views;
using MvvmHelpers;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AresNews.ViewModels
{
    public class NewsViewModel : BaseViewModel
    {
        private bool _isLaunching = true;
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
                OnPropertyChanged(nameof(SearchText));
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
        public Command CloseSearch
        {
            get
            {
                return new Command(() =>
                {
                    IsSearching = false;
                    IsRefreshing = true;
                    FetchArticles();
                });
            }
        }
        public Command Search
        {
            get
            {
                return new Command(() =>
                {
                    IsRefreshing = true;
                    FetchArticles();
                });
            }
        }

        // Property list of articles
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
        // Command to add a Bookmark
        private readonly Command _shareArticle;

        public Command ShareArticle
        {
            get { return _shareArticle; }
        }
        // See detail of the article
        public Command GoToDetail
        {
            get { return new Command(async (id) =>
            {
                var articlePage = new ArticlePage(_articles.FirstOrDefault(art => art.Id == id.ToString()));

                
                await App.Current.MainPage.Navigation.PushAsync(articlePage);
            }); ; }
        }

        private bool _isRefreshing;

        public bool IsRefreshing
        {
            get { return _isRefreshing; }
            set 
            { 
                _isRefreshing = value;
                OnPropertyChanged();
            }
        }
        public NewsViewModel()
        {
            _articles = new ObservableCollection<Article>();
            Xamarin.Forms.BindingBase.EnableCollectionSynchronization(Articles,null, ObservableCollectionCallback);

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

                    throw ex;
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
                       


                   //Articles[_articles.IndexOf(article)] = article;

                   // Say the the bookmark has been updated
                   MessagingCenter.Send<Article>(article, "SwitchBookmark");


               });

            _refreshFeed = new Command( () =>
               {
                   // Fetch the article
                   FetchArticles();
               });

            // Set command to share an article
            _shareArticle = new Command(async (id) =>
           {
               // Get selected article
               var article = _articles.FirstOrDefault(art => art.Id == id.ToString());

               await Share.RequestAsync(new ShareTextRequest
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
        public async void FetchArticles()
        {
            var articles = new Collection<Article>();

            try
            {
                // If we want to fetch the articles via search
                if (_searchText != null && IsSearching == true)
                {
                    SearchArticles(articles);
                    return;
                }

                articles = await App.WService.Get<Collection<Article>>("feeds"/*, parameters: new string[] {DateTime.Now.AddDays(-7).ToString("dd-MM-yyyy"), "now" }*/);

                if (_isLaunching)
                {
                    // Manage backuo
                    _ = Task.Run(() =>
                      {
                          App.BackUpConn.DeleteAll<Article>();
                          App.BackUpConn.InsertAllWithChildren(articles);
                      });
                    _isLaunching = false;

                }


            }
            catch (Exception ex)
            {
                articles = new ObservableCollection<Article> (GetBackupFromDb()) ;

                var page = (NewsPage)((IShellSectionController)Shell.Current?.CurrentItem?.CurrentItem).PresentedPage;
                page.DisplayOfflineMessage();
            }


            // Update list of articles
            Articles = new ObservableCollection<Article>(articles.OrderBy(a => a.Time));

            IsRefreshing = false;

        }
        /// <summary>
        /// Load articles via search
        /// </summary>
        /// <param name="articles"></param>
        private async void SearchArticles(Collection<Article> articles)
        {
            articles = await App.WService.Get<Collection<Article>>("feeds",jsonBody: $"{{\"search\": \"{_searchText}\"}}");

            // Update list of articles
            Articles = new ObservableCollection<Article>(articles);

            IsRefreshing = false;
        }
        /// <summary>
        /// Refresh articles
        /// </summary>
        public async void RefreshArticles ()
        {
            if (_articles.Count > 0)
            {
                //  Refresh articles
                await Task.Run (() => Articles = new ObservableCollection<Article>(_articles));
                
            }

        }
        private static IEnumerable<Article> GetBackupFromDb()
        {
            return App.BackUpConn.GetAllWithChildren<Article>(recursive: true).Reverse<Article>();
        }
        void ObservableCollectionCallback(IEnumerable collection, object context, Action accessMethod, bool writeAccess)
        {
            // `lock` ensures that only one thread access the collection at a time
            lock (collection)
            {
                accessMethod?.Invoke();
            }
        }
    }
}
