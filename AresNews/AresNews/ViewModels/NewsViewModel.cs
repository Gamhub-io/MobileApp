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

                   // Marked the article as saved
                   article.IsSaved = !article.IsSaved;

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
                   Task.Run( () => FetchArticles());
               });

            // Set command to share an article
            _shareArticle = new Command(async (url) =>
           {
               await Share.RequestAsync(new ShareTextRequest
               {
                   Uri = url.ToString(),
                   Title = "Share this article"
               });
           });

        }
        /// <summary>
        /// Fetch all the articles
        /// </summary>
        /// <param name="forceUpdate">force the update of the article list</param>
        /// <returns></returns>
        public void FetchArticles(bool forceUpdate = false)
        {
            //IsRefreshing = true;

            var articles = new ObservableCollection<Article>();

            var sources = App.Sources;

            // move throu all the sources
            foreach (var source in App.Sources)
            //for (int i = 0; i < sources.Count; i++)
            {
                //var source = sources[i];

                // Create the RSS reader
                var reader = XmlReader.Create(source.Url);

                SyndicationFeed feed = SyndicationFeed.Load(reader);

                reader.Close();


                // Add the news article of this feed one by one
                Task.Run(() =>
                {
                    foreach (SyndicationItem item in feed.Items)
                    {
                        var creatorExtension = item.ElementExtensions.FirstOrDefault(e => e.OuterName == "creator" );
                        var encodedExt = item.ElementExtensions.FirstOrDefault(e => e.OuterName == "encoded");
                        
                        string creator = null;
                        string encoded = null;

                        // If the author name is in the extension then we can mention it
                        if (creatorExtension != null)
                            creator = creatorExtension.GetObject<XElement>().Value;
                        
                        if (encodedExt != null)
                            encoded = encodedExt.GetObject<XElement>().Value;

                        // Get the main image
                        var image = GetImagesFromRssItem(item)[0];
                        // include the article only if the link is not an ad and if we can get an image
                        string articleUrl = item.Links[0].Uri.OriginalString;

                        if (!string.IsNullOrEmpty(image) && articleUrl.Contains(source.Domain))
                        {
                            //sstring creator = item.ElementExtensions.FirstOrDefault(e => e.OuterName == "creator").GetObject<XElement>().Value;
                            string id = item.Id;
                            articles.Add(new Article
                            {
                                Id = id,
                                Title = item.Title.Text,
                                Content =  Regex.Replace(item.Summary.Text, "^<img[^>]*>", string.Empty),
                                Author = creator ?? (item.Authors.Count != 0 ? item.Authors[0].Name : string.Empty),
                                FullPublishDate = item.PublishDate.DateTime.ToLocalTime(),
                                SourceName = source.Name,
                                Image = image,
                                Url = articleUrl,
                                IsSaved = App.SqLiteConn.Find<Article>(id) != null

                            });
                        }

                    }
                });
                

                
            }
            IsRefreshing = false;

            // Update list of articles
            Articles = new ObservableCollection<Article>(articles.OrderBy(a => a.Time));


            //Articles = new ObservableCollection<Article>(articles.OrderBy(a => a.Time));
        }
        /// <summary>
        /// Fetch an image from a url
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        private static List<string> GetImagesFromRssItem(SyndicationItem item )
        {
            // Instanciate the list
            List<string> images = new List<string>();

            var contentExtension = item.ElementExtensions.FirstOrDefault(e => e.OuterName == "content");
            var thumnailExtension = item.ElementExtensions.FirstOrDefault(e => e.OuterName == "thumbnail");

            // Search if the image is clearly geven
            if (contentExtension != null)
            {
                AddImageFromExt(contentExtension);
            }
            if (thumnailExtension != null)
            {
                AddImageFromExt(thumnailExtension);
            }
            if (images.Count == 0)
            {
                // Load the Html
                HtmlDocument doc = new HtmlDocument();

                doc.LoadHtml(item.Summary.Text);

                HtmlNodeCollection imageNodes = doc.DocumentNode.SelectNodes("//img");

                if (imageNodes != null && imageNodes.Count != 0)
                {
                    foreach (HtmlNode node in imageNodes)
                    {
                        images.Add(node.Attributes["src"].Value);
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
                            images.Add(src);

                    }
                    images.Add(string.Empty);
                }
            }
            

            return images;

            void AddImageFromExt(SyndicationElementExtension Extension)
            {
                XElement ele = Extension.GetObject<XElement>();

                string value = ele.Attribute(XName.Get("url")).Value;

                if (!string.IsNullOrWhiteSpace(value)/*value != string.Empty*/)
                    images.Add(value);
            }
        }
    }
}
