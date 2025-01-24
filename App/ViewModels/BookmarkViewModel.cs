using CommunityToolkit.Mvvm.Messaging;
using GamHubApp.Models;
using GamHubApp.Services;
using SQLite;
using SQLiteNetExtensions.Extensions;
using System.Collections.ObjectModel;

namespace GamHubApp.ViewModels;

public class BookmarkViewModel : BaseViewModel
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
    
    public BookmarkViewModel()
    {

        Bookmarks = new ObservableCollection<Article>(GetArticlesFromDb());

        // Handle if a article change sees a change of bookmark state
        WeakReferenceMessenger.Default.Register(this, (MessageHandler<object, BookmarkChangedMessage>)((r, m) =>
        {
            if (m.ArticleSent is null)
                return;
            if (m.Saved)
            {
                Bookmarks.Add(m.ArticleSent);
                return;
            }
            Bookmarks.Remove(_bookmarks.SingleOrDefault(bm => bm.MongooseId == m.ArticleSent.MongooseId));
        }));
    }
    /// <summary>
    /// Get all the articles bookmarked from the local database
    /// </summary>
    /// <returns></returns>
    private static IEnumerable<Article> GetArticlesFromDb()
    {
        IEnumerable<Article> articles;
        using (var conn = new SQLiteConnection(App.GeneralDBpath))
        {
            articles = conn.GetAllWithChildren<Article>(recursive: true).Reverse<Article>();
            conn.Close();
        }
        return articles;
    }
}
