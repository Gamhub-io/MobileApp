

using GamHubApp.Views;
using Newtonsoft.Json;
using SQLite;
using SQLiteNetExtensions.Attributes;
using SQLiteNetExtensions.Extensions;

namespace GamHubApp.Models
{
    public class Article
    {
        [PrimaryKey, Column("_id"), JsonProperty("uuid")]
        public string Id { get; set; }
        [Column("mongoId"), JsonProperty("_id")]
        public string MongooseId { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("textSnipet")]
        public string TextSnipet { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
        [ForeignKey(typeof(Source))]
        public int SourceIdFk { get; set; }
        [JsonProperty("sourceId")]
        public string SourceId { get; set; }
        [JsonProperty("blocked")]
        public bool? Blocked { get; set; }
        [JsonIgnore]
        public Source Source { get
                {
                return App.Sources.FirstOrDefault(s => s.MongoId == SourceId);
            } }
        [JsonProperty("isoDate")]
        public DateTime FullPublishDate { get; set; }
        [JsonProperty("categories"), Ignore]
        public string[] Categories { get; set; }
        public string PublishDate
        {
            get
            {
                return FullPublishDate.ToString("dd/MM/yyyy");
            }
        }
        public string PublishTime
        {
            get
            {
                return FullPublishDate.ToLocalTime().ToString("HH:mm");
            }
        }
        public string Url { get; set; }
        public TimeSpan Time
        {
            get
            {
                return DateTime.Now - this.FullPublishDate.ToLocalTime();
            }
        }
        private bool? _isSaved = null;
        public bool IsSaved
        {
            get
            {
                if (_isSaved == null)
                    return App.SqLiteConn.Find<Article>(Id) != null;

                return (bool)_isSaved;
            }
            set { _isSaved = value; }
        }
        /// <summary>
        /// Compare this article to another
        /// </summary>
        /// <param name="otherArticle">the other article you want to compare</param>
        /// <returns></returns>
        public bool IsEqualTo(Article otherArticle)
        {
            if (this == null || otherArticle == null)
            {
                return this == otherArticle;
            }
            foreach (var property in typeof(Article).GetProperties())
            {
                var value1 = property.GetValue(this);
                var value2 = property.GetValue(otherArticle);
                if (!Equals(value1, value2))
                {
                    return false;
                }
            }
            return true;
        }

        [Ignore]
        public Command GoToDetail
        {
            get
            {
                return new Command(() =>
                {
                    var articlePage = new ArticlePage(this);


                    _ = (App.Current as App).Windows[0].Page.Navigation.PushAsync(articlePage);
                }); ;
            }
        }
        [Ignore]
        public Command AddBookmark
        {
            get
            {
                return new Command(() =>
                {
                    // If the article is already in bookmarks
                    bool isSaved = IsSaved;

                    //// Marked the article as saved
                    IsSaved = !IsSaved;

                    if (isSaved)
                        App.SqLiteConn.Delete(this, recursive: true);
                    else
                    {
                        // Insert it in database
                        App.SqLiteConn.InsertWithChildren(this, recursive: true);
                    }

                    // Say the the bookmark has been updated
                    MessagingCenter.Send<Article>(this, "SwitchBookmark");
                }); ;
            }
        }

        [Ignore]
        public Command ShareArticle
        {
            get
            {
                return new Command(() =>
                {
                    _ = Share.RequestAsync(new ShareTextRequest
                    {
                        Uri = Url,
                        Title = "Share this article",
                        Subject = Title,
                        Text = Title
                    });
                }); ;
            }
        }
    }
}