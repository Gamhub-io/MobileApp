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
        public string TimeAgo
        {
            get
            {
                string span = string.Empty;
                TimeSpan timeSpan = DateTime.Now - this.PublishDate;

                // If it's more than a year
                if (timeSpan.Days > 365)
                {
                    // get the number of years
                    int nbYears = timeSpan.Days / 365;

                    span = nbYears.ToString() + " years ago";
                }
                // It's more than a month
                else if (timeSpan.Days > 30)
                {
                    // Number of months
                    int nbMonth = timeSpan.Days / 30;

                    if (nbMonth == 1)
                        span = "1 month ago";
                    else
                        span = nbMonth.ToString() + " months ago";
                }
                // It's more than a week
                else if (timeSpan.Days > 7)
                {
                    // Number of weeks
                    int nbWeeks = timeSpan.Days / 7;

                    if (nbWeeks == 1)
                        span = "1 week ago";
                    else
                        span = nbWeeks.ToString() + " weeks ago";

                }
                // It's more than a day
                else if (timeSpan.Days > 0)
                {
                    int days = timeSpan.Days;

                    if (days == 1)
                        span = "1 day ago";
                    else
                        span = days.ToString() + " days ago";
                }
                // it's more than an hour
                else if (timeSpan.Hours > 0)
                {
                    int hours = timeSpan.Hours;

                    if (hours == 1)
                        span = "1 hour ago";
                    else
                        span = hours.ToString() + " hours ago";
                }
                else if (timeSpan.Minutes > 0)
                {
                    int minutes = timeSpan.Minutes;

                    if (minutes == 1)
                        span = "1 minute ago";
                    else
                        span = minutes.ToString() + " minutes ago";
                }
                else if (timeSpan.Seconds > 0)
                {
                    int seconds = timeSpan.Seconds;
                    if (seconds == 1)
                        span = "1 second ago";
                    else
                        span = seconds.ToString() + " Seconds ago";
                }
                else
                {
                    span = "just now";
                }

                return span;
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
