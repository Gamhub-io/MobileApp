using Newtonsoft.Json;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Text;
using Microsoft.Maui.Controls;
using Microsoft.Maui;

namespace GamHub.Models
{
    public class Feed : SelectableModel
    {
        [PrimaryKey, Column("_id")]
        public string Id { get; set; }
        public string Title { get; set; }

        private string _keywords;
        public string Keywords
        {
            get { return _keywords; }
            set
            {
                //if (_isSelected != value)
                //{
                _keywords = value;
                OnPropertyChanged(nameof(Keywords));
                //}
            }
        }
        public bool IsSaved { get; set; }
        [Ignore]
        public bool IsLoaded { get; set; }
        [Ignore]
        public Xamarin.Forms.Color ButtonColor { get; set; } = (Xamarin.Forms.Color)Application.Current.Resources["LightDark"];
        [Ignore]
        public ObservableCollection<Article> Articles { get; set; }

    }
}
