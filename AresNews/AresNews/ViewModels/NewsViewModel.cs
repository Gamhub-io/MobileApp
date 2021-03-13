using HtmlAgilityPack;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Syndication;
using System.Xml;

namespace AresNews.ViewModels
{
    public class NewsViewModel : BaseViewModel
    {
        private ObservableCollection<SyndicationItem> _articles;

        public ObservableCollection<SyndicationItem> Articles
        {
            get { return _articles; }
            set 
            { 
                _articles = value;
                OnPropertyChanged();
            }
        }

        public NewsViewModel()
        {
            _articles = new ObservableCollection<SyndicationItem>();

            FetchArticles();



        }
        private void FetchArticles()
        {
            // move throu all the sources
            foreach (var source in App.Sources)
            {
                // Create the RSS reader
                var reader = XmlReader.Create(source.Url);

                SyndicationFeed feed = SyndicationFeed.Load(reader);

                
                reader.Close();

                // Add the news article of this feed one by one
                foreach (SyndicationItem item in feed.Items)
                {
                    Articles.Add(item);
                }
            }
        }
        private static List<string> GetImagesFromHTML(string html)
        {
            // 
            List<string> images = new List<string>();

            // Load the Html
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Now, using LINQ to get all Images
            List<HtmlNode> imageNodes = null;
            imageNodes = (from HtmlNode node in doc.DocumentNode.SelectNodes("//img")
                          where node.Name == "img"
                          && node.Attributes["class"] != null
                          && node.Attributes["class"].Value.StartsWith("img_")
                          select node).ToList();

            foreach (HtmlNode node in imageNodes)
            {
                images.Add(node.Attributes["src"].Value);
            }

            return images;
        }
    }
}
