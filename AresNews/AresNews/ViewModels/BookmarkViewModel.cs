using AresNews.Models;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using Xamarin.Forms;

namespace AresNews.ViewModels
{
    public class BookmarkViewModel :BaseViewModel
    {
        // Property list of articles
        private ObservableCollection<Article> _bookmarks;

        public ObservableCollection<Article> Bookmarks
        {
            get { return _bookmarks; }
            set
            {
                _bookmarks = value;
                OnPropertyChanged();
            }
        }
        // Command to add a Bookmark
        private Command _addBookmark;

        public Command AddBookmark
        {
            get { return _addBookmark; }
        }
        public BookmarkViewModel()
        {

            Bookmarks = new ObservableCollection<Article>(App.SqLiteConn.Table<Article>().ToList());


            _addBookmark = new Command((id) =>
            {
                //App.StartDb();

                // Get the article
                var article = _bookmarks.FirstOrDefault(art => art.Id == id.ToString());
                
                App.SqLiteConn.Delete(article);

                // Marked the article as saved
                Bookmarks.Remove(article);


            });
        }
    }
}
