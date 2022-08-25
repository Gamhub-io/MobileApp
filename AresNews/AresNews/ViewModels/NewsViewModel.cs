using AresNews.Models;
using AresNews.Services;
using AresNews.Views;
using MvvmHelpers;
using Newtonsoft.Json;
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
        private string _prevSearch;
        private string _lastCallDateTime;

        private int _wifiRestartCount = 0;
        private bool _isInCustomFeed;

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
                    // Scroll up before fetching the items
                    CurrentPage.ScrollFeed();
                    IsSearching = false;
                    IsRefreshing = true;
                    _prevSearch = null;
                    //FetchArticles(true);

                });
            }
        }
        public Command Search
        {
            get
            {
                return new Command(() =>
                {
                    //if (_searchText == _prevSearch)
                        //return;
                    IsRefreshing = true;
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
                SetProperty(ref _articles, value);
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
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }
        public NewsViewModel(NewsPage currentPage)
        {
            CurrentPage = currentPage;
            Articles = new ObservableRangeCollection<Article>(GetBackupFromDb().OrderBy(a => a.Time).ToList());
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
                   if (IsSearchOpen)
                   {
                       Search.Execute(null);
                   }
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

            //
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    _refreshFeed.Execute(null);
                    break;
                case Device.Android:
                      IsRefreshing = true;
                    FetchArticles();
                    break; 
                default:
                    break;
            }

        }

        /// <summary>
        /// Fetch all the articles
        /// </summary>
        public async void FetchArticles(bool isFullRefresh = false)
        {

            var articles = new ObservableCollection<Article>();
            if (isFullRefresh)
            {
                //await Task.Run(async () =>
                //{

                articles = await App.WService.Get<ObservableCollection<Article>>("feeds");
                
                _isLaunching = false;
                Articles = articles;
                // Register date of the refresh
                //Preferences.Set("lastRefresh", _articles[0].FullPublishDate.ToString("dd-MM-yyy_HH:mm"));
                IsRefreshing = false;
                IsSearchOpen = false;
                _prevSearch = string.Empty;
                return;
                //});
            }



            try
            {
                if (_articles?.Count() > 0)
                    _lastCallDateTime = _articles?.First().FullPublishDate.ToUniversalTime().ToString("dd-MM-yyy_HH:mm:ss");
                ;
                // If we want to fetch the articles via search
                if (!string.IsNullOrEmpty(_searchText) && IsSearching == true )
                {
                    SearchArticles(articles.ToList());
                    return;
                }
                if (string.IsNullOrEmpty(_lastCallDateTime))
                {
                    
                    Articles = await App.WService.Get<ObservableCollection<Article>>("feeds");
                    IsRefreshing = false;
                    _isLaunching = false;
                    await RefreshDB();
                    return ;
                }
               if (_articles?.Count() > 0)
               {
                   
                       articles = await App.WService.Get<ObservableCollection<Article>>("feeds", "update", parameters: new string[] { _lastCallDateTime });

                    

                }
               else
               {
                   if (_isLaunching)
                    {
                        try
                        {
                            Articles = await App.WService.Get<ObservableCollection<Article>>("feeds");

                            // Manage backuo
                            await RefreshDB();

                        }
                        catch
                        {
                        }

                        _isLaunching = false;
                        IsRefreshing = false;
                        return;
                    }
                   articles = await App.WService.Get<ObservableCollection<Article>>("feeds");

               }
            }
            catch (Exception ex)
            {
                // BLAME: the folowing lines are disgusting but it works 
                // TODO: change this if possible
                if (ex.Message.Contains("Network subsystem is down") && Device.RuntimePlatform == Device.Android && _wifiRestartCount < 3)
                {
                    // Restart wifi: only works with android < Q
                    if (DependencyService.Get<IInternetManagement>().TurnWifi(false))
                    {
                        DependencyService.Get<IInternetManagement>().TurnWifi(true);

                        _wifiRestartCount++;

                        // call the thing again
                        FetchArticles();
                        return;

                    }
                }
                //articles = new ObservableCollection<Article> (GetBackupFromDb().OrderBy(a => a.Time).ToList()) ;

                var page = (NewsPage)((IShellSectionController)Shell.Current?.CurrentItem?.CurrentItem).PresentedPage;
                page.DisplayOfflineMessage(ex.Message);
            }

            // To avoid crashs: if this number is out of range we end the process
            if (articles == null || articles.Count() <= 0)
            {
                IsRefreshing = false;
                return;
            }

            await Task.Factory.StartNew(() =>
            {
                // Update list of articles
                UpdateArticles(articles);
            });
                try
            {
                // Manage backuo
                await RefreshDB();

            }
            catch
                {
                }
                //finally
                //{
                //    _isLaunching = false;
                //}

            _isLaunching = false;
            IsRefreshing = false;


            // Register date of the refresh
            Preferences.Set("lastRefresh", _articles[0].FullPublishDate.ToString("dd-MM-yyy_HH:mm"));


        }

        private void UpdateArticles(ObservableCollection<Article> articles)
        {
            for (int i = 0; i < articles.Count(); i++)
            {
                var current = articles[i];
                Article existingArticle = _articles.FirstOrDefault(a => a.Id == current.Id);
                if (existingArticle == null)
                {

                    Article item = articles.FirstOrDefault(a => a.Id == current.Id);

                    var index = articles.IndexOf(item);

                    // Add article one by one for a better visual effect
                    Articles.Insert(index == -1 ? 0 + i : index, current);
                }
                else
                {
                    int index = _articles.IndexOf(existingArticle);
                    // replace the exisiting one with the new one
                    Articles.Remove(existingArticle);
                    Articles.Insert(index, current);
                }


            }
        }

        private async Task RefreshDB()
        {
            await Task.Factory.StartNew(async () =>
            {
                await Task.Run(() =>
                {

                    App.BackUpConn.DeleteAll<Article>();
                    App.BackUpConn.InsertAllWithChildren(_articles);
                });
            });
        }

        /// <summary>
        /// Load articles via search
        /// </summary>
        /// <param name="articles"></param>
        private async void SearchArticles(List<Article> articles)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                
                articles = await App.WService.Get<List<Article>>("feeds", jsonBody: $"{{\"search\": \"{_searchText}\"}}");
                
            }
            // Offline search
            else
            {
                var words = _searchText.Split(' ');
                for (int i = 0; i < words.Count(); i++)
                {
                    var word = words[i];
                    articles.AddRange(_articles.Where((e) => e.Title.Contains(word)));
                }
                //articles.AddRange = _articles.Where((e) => e.Title.Contains(_searchText.Split()) || e.Content.Contains())
            }
            // Update list of articles
            Articles = new ObservableCollection<Article>(articles);

            IsRefreshing = false;
            _isInCustomFeed = true;
            IsSearchOpen = true;
            _prevSearch = _searchText;

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
