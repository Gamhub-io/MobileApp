using CommunityToolkit.Mvvm.Messaging;
using GamHubApp.Models;
using GamHubApp.Services;
using System.Collections.ObjectModel;

namespace GamHubApp.ViewModels;

public class BookmarkViewModel : BaseViewModel
{
    // Property list of articles
    private ObservableCollection<Article> _bookmarks;

    public App CurrentApp { get; }

    private GeneralDataBase _generalDB;

    public ObservableCollection<Article> Bookmarks
    {
        get { return _bookmarks; }
        set
        {
            _bookmarks = value;
            OnPropertyChanged();
        }
    }
    
    public BookmarkViewModel(GeneralDataBase generalDataBase)
    {
        // CurrentApp and CurrentPage will allow use to access to global properties
        CurrentApp = App.Current as App;
        CurrentApp.ShowLoadingIndicator();
        _generalDB = generalDataBase;
        _ = LoadBookmarksFromDb();

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
        CurrentApp.RemoveLoadingIndicator();

    }

    /// <summary>
    /// Get all the articles bookmarked from the local database
    /// </summary>
    /// <returns></returns>
    private async Task LoadBookmarksFromDb()
    {
        Bookmarks = new ObservableCollection<Article>(await _generalDB.GetBookmarkedArticles());
    }
}
