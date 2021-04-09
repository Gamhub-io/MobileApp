using AresNews.Models;
using AresNews.Views;
using HtmlAgilityPack;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.ServiceModel;
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

            _addBookmark = new Command((id) =>
               {
                   //App.StartDb();

                   // Get the article
                   var article = _articles.FirstOrDefault(art => art.Id == id.ToString());



                   // If the article is already in bookmarks
                   bool isSaved = article.IsSaved;

                   //// Marked the article as saved
                   //article.IsSaved = !article.IsSaved;

                   if (isSaved)
                       App.SqLiteConn.Delete(article);
                   else
                       // Insert it in database
                       App.SqLiteConn.Insert(article);


                   Articles[_articles.IndexOf(article)] = article;

                   
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
        /// <param name="forceUpdate">force the update of the article list</param>
        /// <returns></returns>
        public async void FetchArticles(bool forceUpdate = false)
        {

            var articles = new ObservableCollection<Article>();

            var sources = App.Sources;

            await Task.Run(
                () =>
                {
                    // Move throu all the sources
                    //foreach (var source in App.Sources)
                    for (int i = 0; i < sources.Count; i++)
                    {
                        var source = sources[i];

                        SyndicationFeed feed;

                        // Create the RSS reader
                        using (var reader = XmlReader.Create(source.Url))
                        {
                            feed = SyndicationFeed.Load(reader);

                        }

                        if (feed == null)
                            return;

                        // Add the news article of this feed one by one
                        foreach (SyndicationItem item in feed.Items)
                        {

                            // include the article only if the link is not an ad and if we can get an image
                            string articleUrl = item.Links[0].Uri.OriginalString;

                            // Ensure that the article is not an ad
                            if (articleUrl.Contains(source.Domain))
                            {
                                // Get the main image
                                var image = GetImagesFromRssItem(item);

                                // Show only the article containing a potential thumbnail
                                if (!string.IsNullOrEmpty(image))
                                {

                                    var creatorExtension = item.ElementExtensions.FirstOrDefault(e => e.OuterName == "creator");
                                    var encodedExt = item.ElementExtensions.FirstOrDefault(e => e.OuterName == "encoded");

                                    string creator = null;
                                    string encoded = null;

                                    // If the author name is in the extension then we can mention it
                                    if (creatorExtension != null)
                                        creator = Sterilize(creatorExtension.GetObject<XElement>().Value);

                                    if (encodedExt != null)
                                        encoded = encodedExt.GetObject<XElement>().Value;


                                    // Add the Article
                                    articles.Add(new Article
                                    {
                                        Id = item.Id,
                                        Title = item.Title.Text,
                                        Content = string.IsNullOrEmpty(encoded) ? Regex.Replace(Sterilize(item.Summary.Text), "^<img[^>]*>", string.Empty) : Regex.Replace(encoded, "(\\[.*\\])", string.Empty),
                                        Author = creator ?? (item.Authors.Count != 0 ? item.Authors[0].Name : string.Empty),
                                        FullPublishDate = item.PublishDate.DateTime.ToLocalTime(),
                                        SourceName = source.Name,
                                        Image = image,
                                        Url = articleUrl,

                                    });
                                }
                            }


                        }


                    }
                    
                }
            ).ContinueWith((r) =>
            {
                IsRefreshing = false;

                // Update list of articles
                Articles = new ObservableCollection<Article>(articles.OrderBy(a => a.Time));
            });

        }
        /// <summary>
        /// Function to remove polutions in a string
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string Sterilize(string text)
        {
            return Regex.Replace(text, @"[\r|\n|\t]", string.Empty);
        }
        /// <summary>
        /// Fetch an image from a url
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static string GetImagesFromRssItem(SyndicationItem item )
        {
            // Instanciate the list
            //List<string> images = new List<string>();

            var contentExtension = item.ElementExtensions.FirstOrDefault(e => e.OuterName == "content");
            var thumnailExtension = item.ElementExtensions.FirstOrDefault(e => e.OuterName == "thumbnail");
            //var thumnailsExtension = item.ElementExtensions.FirstOrDefault(e => e.OuterName == "thumbnails");
            var imageExtension = item.ElementExtensions.FirstOrDefault(e => e.OuterName == "image");

            // in case the image is in a image block
            if (imageExtension != null)
            {
                var el = imageExtension.GetObject<XElement>();

                var img =  el.Element("url").Value;

                // Add the image
                if (!string.IsNullOrEmpty(img))
                    return img;

            } else if (contentExtension != null )
            {
                return AddImageFromExt(contentExtension);
            }
            // Search if the image is clearly geven

            if (thumnailExtension != null)
            {
                return AddImageFromExt(thumnailExtension);
            }
            
            //if (images.Count == 0)
            //{
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
                else
                {
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
                }
            //}
            

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
    }
}
