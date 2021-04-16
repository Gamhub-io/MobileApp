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
                return DateTime.Now.ToUniversalTime() - this.FullPublishDate.ToUniversalTime();
            } }
        //public string TimeAgo
        //{
        //    get
        //    {
        //        string span;

        //        // If it's more than a year
        //        if (this.Time.Days > 365)
        //        {
        //            // get the number of years
        //            int nbYears = Time.Days / 365;

        //            span = $"{nbYears} years ago";
        //        }
        //        // It's more than a month
        //        else if (Time.Days > 30)
        //        {
        //            // Number of months
        //            int nbMonth = Time.Days / 30;

        //            if (nbMonth == 1)
        //                span = "1 month ago";
        //            else
        //                span = $"{nbMonth} months ago";
        //        }
        //        // It's more than a week
        //        else if (Time.Days > 7)
        //        {
        //            // Number of weeks
        //            int nbWeeks = Time.Days / 7;

        //            if (nbWeeks == 1)
        //                span = "1 week ago";
        //            else
        //                span = $"{nbWeeks} weeks ago";

        //        }
        //        // It's more than a day
        //        else if (Time.Days > 0)
        //        {
        //            int days = Time.Days;

        //            if (days == 1)
        //                span = "1 day ago";
        //            else
        //                span = $"{days} days ago";
        //        }
        //        // it's more than an hour
        //        else if (Time.Hours > 0)
        //        {
        //            int hours = Time.Hours;

        //            if (hours == 1)
        //                span = "1 hour ago";
        //            else
        //                span = $"{hours} hours ago";
        //        }
        //        else if (Time.Minutes > 0)
        //        {
        //            int minutes = Time.Minutes;

        //            if (minutes == 1)
        //                span = "1 minute ago";
        //            else
        //                span = $"{minutes} minutes ago";
        //        }
        //        else if (Time.Seconds > 0)
        //        {
        //            int seconds = Time.Seconds;
        //            if (seconds == 1)
        //                span = "1 second ago";
        //            else
        //                span = $"{seconds} Seconds ago";
        //        }
        //        else
        //        {
        //            span = "just now";
        //        }

        //        return span;
        //    }
        //}
        public bool IsSaved { get { return App.SqLiteConn.Find<Article>(Id) != null; } }
    }
}
