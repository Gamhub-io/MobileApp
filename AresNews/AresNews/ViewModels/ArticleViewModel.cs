using AresNews.Models;
using MvvmHelpers;
using SQLiteNetExtensions.Extensions;
using System.Diagnostics;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace AresNews.ViewModels
{
    public class ArticleViewModel : BaseViewModel
    {
        private Article _selectedArticle;

        public Article SelectedArticle
        {
            get { return _selectedArticle; }
            set 
            { 
                _selectedArticle = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// Time spend reading the article
        /// </summary>
        public Stopwatch timeSpent { get; set; }

        // Command to add a Bookmark
        private Command _addBookmark;

        public Command AddBookmark
        {
            get { return _addBookmark; }
        }
        // Command to add a Bookmark
        private Command _shareArticle;

        public Command ShareArticle
        {
            get { return _shareArticle; }
        }
        public Command Browse 
        { 
            get 
            {
                return new Command(async () => await Browser.OpenAsync(_selectedArticle.Url, new BrowserLaunchOptions
                {
                    LaunchMode = BrowserLaunchMode.SystemPreferred,
                    TitleMode = BrowserTitleMode.Default,
                }));
            }
        }

        public ArticleViewModel(Article article)
        {

            _addBookmark = new Command((id) =>
            {
                // If the article is already in bookmarks
                bool isSaved = _selectedArticle.IsSaved;

                // Marked the article as saved
                SelectedArticle.IsSaved = !isSaved;


                if (isSaved)
                    App.SqLiteConn.Delete(_selectedArticle);
                else
                    // Insert it in database
                    App.SqLiteConn.InsertWithChildren(_selectedArticle, recursive: true);

                MessagingCenter.Send<Article>(_selectedArticle, "SwitchBookmark");


            });


            // Set command to share an article
            _shareArticle = new Command(async (url) =>
            {
                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = url.ToString(),
                    Title = "Share this article",
                    Subject =_selectedArticle.Title,
                    Text = _selectedArticle.Title
                    
                });
            });

            _selectedArticle = article;

            timeSpent = new Stopwatch();
            timeSpent.Start();
        }
    }
}
