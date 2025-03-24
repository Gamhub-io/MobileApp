using GamHubApp.Core;
using GamHubApp.Models;
using SQLite;

namespace GamHubApp.Services;

public sealed class GaneralDataBase
{
    SQLiteAsyncConnection database;
    public List<Source> Sources { get; private set; }
    public Task Init()
    {
        if (database is not null)
            return null;

        database = new SQLiteAsyncConnection(AppConstant.GeneralDBpath, AppConstant.Flags);
        var creatingSource = database.CreateTableAsync<Source>();
        var creatingArticle = database.CreateTableAsync<Article>();
        var creatingFeed = database.CreateTableAsync<Feed>();


        return Task.WhenAll(creatingSource, creatingArticle, creatingFeed);
    }

    /// <summary>
    /// Get a specific article
    /// </summary>
    /// <param name="id">id of the article</param>
    /// <returns>the article</returns>
    public async Task<Article> GetArticleById(string id)
    {
        await Init();
        return await database.Table<Article>().FirstOrDefaultAsync(a => a.Id == id);
    }

    /// <summary>
    /// Get a specific source
    /// </summary>
    /// <param name="id">id of the source</param>
    /// <returns>the source</returns>
    public async Task<Source> GetSourceById(string id)
    {
        await Init();
        return await database.Table<Source>().FirstOrDefaultAsync(a => a.MongoId == id);
    }

    /// <summary>
    /// Add a specific article to bookmarks
    /// </summary>
    /// <param name="article">the article we want to insert</param>
    /// <returns>update status</returns>
    public async Task<int> InsertArticleBookmark(Article article)
    {
        await Init();
        return await database.InsertOrReplaceAsync(article);
    }

    /// <summary>
    /// Delete a specific article that have been bookmarked
    /// </summary>
    /// <param name="article">the article we want to remove</param>
    /// <returns>update status</returns>
    public async Task<int> DeleteArticleBookmark(Article article)
    {
        await Init();
        return await database.DeleteAsync(article);
    }

    /// <summary>
    /// Get Articles that have been bookmarked
    /// </summary>
    /// <returns>Bookmarked articles</returns>
    public async Task<List<Article>> GetBookmarkedArticles()
    {
        await Init();
        List<Article> articles = new();

        return (await database.Table<Article>().ToListAsync()).Reverse<Article>().ToList();
    }

    /// <summary>
    /// Update a feed
    /// </summary>
    /// <param name="feed">Feed we want to update</param>
    /// <returns>Update status</returns>
    public async Task<int> UpdateFeed(Feed feed)
    {
        await Init();
        return await database.UpdateAsync(feed);
    }

    /// <summary>
    /// Get all feeds
    /// </summary>
    /// <returns>all feeds</returns>
    public async Task<List<Feed>> GetFeeds(Feed feed)
    {
        await Init();
        return await database.Table<Feed>().ToListAsync();
    }

    /// <summary>
    /// insert a feed
    /// </summary>
    /// <param name="feed">Feed we want to insert</param>
    /// <returns>all feeds</returns>
    public async Task<int> InsertFeeds(Feed feed)
    {
        await Init();
        return await database.InsertAsync(feed);
    }


}
