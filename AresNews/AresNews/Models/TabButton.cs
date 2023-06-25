using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace AresNews.Models
{
    public class TabButton : INotifyPropertyChanged
    {
        private string _id;
        private string _title;
        private Color _backgroundColour = (Color)Application.Current.Resources["LightDark"];

        public string Id
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    OnPropertyChanged(nameof(Id));
                }
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }

        public Color BackgroundColour
        {
            get { return _backgroundColour; }
            set
            {
                if (_backgroundColour != value)
                {
                    _backgroundColour = value;
                    OnPropertyChanged(nameof(BackgroundColour));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
