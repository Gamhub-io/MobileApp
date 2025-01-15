using GamHubApp.Models;
using System.Collections.ObjectModel;

namespace GamHubApp.ViewModels
{
    public class BookmarkViewModel : BaseViewModel
    {
        // Property list of articles
        private ObservableCollection<Article> _bookmarks;
        private App _curr;

        public ObservableCollection<Article> Bookmarks
        {
            get { return _bookmarks; }
            set
            {
                _bookmarks = value;
                OnPropertyChanged();
            }
        }
        
        public BookmarkViewModel()
        {

            Bookmarks = new ObservableCollection<Article>(GetArticlesFromDb());

            // Handle if a article change sees a change of bookmark state
            MessagingCenter.Subscribe<Article>(this, "SwitchBookmark", (sender) =>
            {
                try
                {

                    Bookmarks = new ObservableCollection<Article>(GetArticlesFromDb());


                }
                catch (Exception ex)
                {

                    throw ex;
                }


            });
            _addBookmark = new Command((id) =>
            {
                try
                {

                    // Get the article
                    var article = _bookmarks.FirstOrDefault(art => art.Id == id.ToString());

                    App.SqLiteConn.Delete(article, recursive: true);

                    // Say the the bookmark has been removed
                    MessagingCenter.Send<Article>(article, "SwitchBookmark");

                    // Marked the article as saved
                    Bookmarks.Remove(article);

                }
                catch (Exception ex)
                {
#if DEBUG
                    throw ex;
#endif
                }
                finally
                {

                }


            });


            // Set command to share an article
            _shareArticle = new Command(async (id) =>
            {
                // Get selected article
                var article = _bookmarks.FirstOrDefault(art => art.Id == id.ToString());

                await Share.RequestAsync(new ShareTextRequest
                {
                    Uri = article.Url,
                    Title = "Share this article",
                    Subject = article.Title,
                    Text = article.Title
                });
            });
        }
        /// <summary>
        /// Get all the articles bookmarked from the local database
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<Article> GetArticlesFromDb()
        {
            return App.SqLiteConn.GetAllWithChildren<Article>(recursive: true).Reverse<Article>();
        }
    }
}
