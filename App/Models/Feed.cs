
using SQLite;
using System.Collections.ObjectModel;
using Color = Microsoft.Maui.Graphics.Color;

namespace GamHubApp.Models
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
        public Color ButtonColor { get; set; } = (Color)Application.Current.Resources["LightDark"];
        [Ignore]
        public ObservableCollection<Article> Articles { get; set; }

    }
}
