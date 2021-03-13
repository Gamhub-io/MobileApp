using System;
using System.Collections.Generic;
using System.Text;

namespace AresNews.Models
{
    public class Article
    {
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
    }
}
