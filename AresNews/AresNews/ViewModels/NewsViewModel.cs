using AresNews.Models;
using AresNews.Views;
using HtmlAgilityPack;
using MvvmHelpers;
using SQLiteNetExtensions.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AresNews.ViewModels
{
    public class NewsViewModel : BaseViewModel
    {
        // Property list of articles
        private ObservableCollection<Article> _articles;

        public ObservableCollection<Article> Articles
        {
            get { return _articles; }
            set 
            { 
                _articles = value;
                OnPropertyChanged();
            }
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
            var articles = new ObservableCollection<Article>((IEnumerable<Article>)await App.WService.Get<IEnumerable<Article>>("feeds"));


            // Update list of articles
            Articles = new ObservableCollection<Article>(articles.OrderBy(a => a.Time));

            IsRefreshing = false;

        }
        /// <summary>
        /// Function to remove polutions in a string
        /// </summary>
        /// <param name="text">text to serilize</param>
        /// <returns></returns>
        private static string Sterilize(string text)
        {
            return Regex.Replace(text, @"[\r|\n|\t]", string.Empty);
        }
        /// <summary>
        /// Fetch an image for the article
        /// </summary>
        /// <param name="item">Item relative to the article </param>
        /// <returns></returns>
        private static string GetImagesFromRssItem(SyndicationItem item, Dictionary<string, SyndicationElementExtension> extensions)
        {

            var contentExtension = extensions["content"];
            var thumnailExtension = extensions["thumbnail"];
            var imageExtension = extensions["image"];

            // in case the image is in a image block
            if (imageExtension != null)
            {
                var el = imageExtension.GetObject<XElement>();

                var img =  el.Element("url").Value;

                // Add the image
                if (!string.IsNullOrEmpty(img))
                    return img;

            }
            if (contentExtension != null )
            {
                return AddImageFromExt(contentExtension);
            }
            // Search if the image is clearly geven

            if (thumnailExtension != null)
            {
                return AddImageFromExt(thumnailExtension);
            }
                // Load the Html
                HtmlDocument doc = new HtmlDocument();

                doc.LoadHtml(item.Summary.Text);

                HtmlNodeCollection imageNodes = doc.DocumentNode.SelectNodes("//img");

                if (imageNodes != null && imageNodes.Count != 0)
                {
                    foreach (HtmlNode node in imageNodes)
                    {
                        string value = node.Attributes["src"].Value;

                        if (!string.IsNullOrEmpty(value))
                            return value;
                    }
                }
                
                // Load the webpage html
                using (WebClient client = new WebClient())
                {
                    doc.LoadHtml(client.DownloadString(item.Links[0].Uri.OriginalString));

                }

                // Now, using LINQ to get all Images
                imageNodes = doc.DocumentNode.SelectNodes("//img");

                foreach (HtmlNode node in imageNodes)
                {
                    var src = node.Attributes["src"].Value;
                    if (src.Contains(".png") || src.Contains(".jpg") || src.Contains(".jpeg"))
                        return src;

                }
                
                
            

            return string.Empty;

            string AddImageFromExt(SyndicationElementExtension Extension)
            {
                XElement ele = Extension.GetObject<XElement>();

                string value = ele.Attribute(XName.Get("url")).Value;

                if (!string.IsNullOrWhiteSpace(value)/*value != string.Empty*/)
                    return value;

                return string.Empty;
            }

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
        /// <summary>
        /// Merge two list of item without using LINQ
        /// </summary>
        /// <param name="receivers">List which will be returned</param>
        /// <param name="mergers">List to merge</param>
        /// <returns></returns>
        private List<SyndicationItem> Merge (List<SyndicationItem> receivers, List<SyndicationItem> mergers)
        {
            for (int i = 0; i < mergers.Count; i++)
            {
                receivers.Add(mergers[i]);
            }

            return receivers;
        }
        /// <summary>
        /// Get all the extensions from a collection of syndication extension
        /// </summary>
        /// <param name="elementExtensions">collection of syndication extension</param>
        /// <returns>all the relevant extensions</returns>
        private static Dictionary<string,SyndicationElementExtension> GetExtensions(SyndicationElementExtensionCollection elementExtensions)
        {
            var listResult = new Dictionary<string, SyndicationElementExtension>();

            listResult.Add("content", null);
            listResult.Add("thumbnail", null);
            listResult.Add("image", null);
            listResult.Add("encoded", null);
            listResult.Add("creator", null);

            // loop every single extension
            for (int i = 0; i < elementExtensions.Count; i++)
            {
                string outerName = elementExtensions[i].OuterName;

                if (outerName == "content" ||
                    outerName == "thumbnail" ||
                    outerName == "encoded" ||
                    outerName == "creator" ||
                    outerName == "image"
                    )
                    listResult[outerName] = elementExtensions[i];
            }
            
            return listResult;
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
