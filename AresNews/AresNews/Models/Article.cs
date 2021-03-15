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
        public string Image { get; set; }
        public DateTime PublishDate { get; set; }
        public string Url { get; set; }
        public TimeSpan TimeAgo
        {
            get
            {
                return DateTime.Now - PublishDate;
            }
        }
        //public bool IsSaved
        //{
        //    get
        //    {

        //        App.StartDb();
        //        var article = App.SqLiteConn.Find<Article>(Id);
        //        App.CloseDb();

        //        return article != null || _isSaved;
        //    }
        //    set { _isSaved = value; }
        //}
        public bool IsSaved { get; set; }
    }
}
