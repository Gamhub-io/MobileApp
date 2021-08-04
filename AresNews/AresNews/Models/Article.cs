using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace AresNews.Models
{
    public class Article
    {
        [PrimaryKey, Column("_id")]
        public string Id { get; set; }
        [JsonProperty("sourceName")]
        public string SourceName { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("content")]
        public string Content { get; set; }
        [JsonProperty("author")]
        public string Author { get; set; }
        [JsonProperty("image")]
        public string Image { get; set; }
        [JsonProperty("isoDate")]
        public DateTime FullPublishDate { get; set; }
        public string PublishDate { get 
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
        public TimeSpan Time { get 
            {
                return DateTime.Now - this.FullPublishDate.ToLocalTime();
            } 
        }
        private bool ?_isSaved = null;
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
    }
}
