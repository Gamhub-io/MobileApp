using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using Xamarin.Forms;

namespace AresNews.Models
{
    public class Feed
    {
        [PrimaryKey, Column("_id")]
        public string Id { get; set; }
        public string Title { get; set; }
        public string Keywords { get; set; }
        public bool IsSaved { get; set; }
        [Ignore]
        public bool IsLoaded { get; set; }
        [Ignore]
        public bool IsSelected { get; set; }
        [Ignore]
        public Xamarin.Forms.Color ButtonColor { get; set; } = (Xamarin.Forms.Color)Application.Current.Resources["LightDark"];
        [Ignore]
        public ObservableCollection<Article> Articles { get; set; }

    }
}
