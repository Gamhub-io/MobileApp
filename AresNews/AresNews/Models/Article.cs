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
        public string SourceName { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public string Image { get; set; }
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
                return FullPublishDate.ToString("HH:mm");
            }
        }
        public string Url { get; set; }
        public TimeSpan Time { get 
            {
                return DateTime.Now - this.FullPublishDate;
            } 
        }
        public bool IsSaved { get; set; }//{ get { return App.SqLiteConn.Find<Article>(Id) != null; } }
    }
}
