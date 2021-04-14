using AresNews.Models;
using AresNews.Views;
using HtmlAgilityPack;
using MvvmHelpers;
using System;
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
                async () =>
                {
                    var items = new List<SyndicationItem>();

                    // Move throu all the sources
                    //foreach (var source in App.Sources)
                    await Task.Run(async () => 
                    {
                        try
                        {
                            for (int i = 0; i < sources.Count; i++)
                            {
                                await Task.Run( () => 
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

                                    // Override feed title
                                    feed.Title = new TextSyndicationContent(source.Name);

                                    // Add the domain
                                    feed.Items.Select(it =>
                                    {
                                        it.BaseUri = new Uri(source.Url);
                                        it.SourceFeed = feed;
                                        return it;
                                    }).ToList();

                                    //items = items.Concat(feed.Items).ToList();
                                    items = Merge(items, new List<SyndicationItem>(feed.Items));

                                });
                                


                            }
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }).ContinueWith((res)=> 
                    {
                        try
                        {
                            // Add the news article of this feed one by one
                            for (int i = 0; i < items.Count; i++)
                            {
                                var item = items[i];

                                // Ensure that the article is not an ad
                                if (item.BaseUri.Host == item.Links[0].Uri.Host)
                                {
                                    // Get the extensions
                                    var extensions = GetExtensions(item.ElementExtensions);

                                    // Get the main image
                                    var image = GetImagesFromRssItem(item, extensions);

                                    // Show only the article containing a potential thumbnail
                                    if (!string.IsNullOrEmpty(image))
                                    {

                                        var creatorExtension = extensions["creator"];
                                        var encodedExt = extensions["encoded"];

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
                                            SourceName = item.SourceFeed.Title.Text,
                                            Image = image,
                                            Url = item.Links[0].Uri.OriginalString,

                                        });
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                            throw;
                        }
                    });
                    

                    
                    
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
    }
}
