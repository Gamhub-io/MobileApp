using GamHubApp.Core;
using GamHubApp.Models;
using GamHubApp.Models.Http.Payloads;
using GamHubApp.Models.Http.Responses;
using CustardApi.Objects;
using Newtonsoft.Json;
using System.Collections.ObjectModel;

#if DEBUG
using System.Diagnostics;
#endif

namespace GamHubApp.Services;

public class Fetcher
{
    public static string ProdHost { get; } = "api.gamhub.io";
    private static string _dateFormat = "dd-MM-yyy_HH:mm:ss";
    private Session CurrentSession { get; set; }
    public Service WebService { get; private set; }
    public static Collection<Source> Sources { get; set; }

    private GeneralDataBase _generalDB;
    private BackUpDataBase _backupDB;
    private Collection<Deal> _deals;

    public User UserData { get; set; }
    public List<Article> Bookmarks { get; private set; }


    public Fetcher(GeneralDataBase generalDataBase, BackUpDataBase backUpDataBase)
    {
#if DEBUG_LOCALHOST
        // Set webservice
        WebService = new Service(host: AppConstant.Localhost,
                                port: 255,
                               sslCertificate: false);
#else
        // Set webservice
        WebService = new Service(host: ProdHost,
                               sslCertificate: true);
#endif
        _generalDB = generalDataBase;
        _backupDB = backUpDataBase;
  
        GetSources().GetAwaiter();
    }

    /// <summary>
    /// Get the last 2 months worth of feed
    /// </summary>
    /// <returns>last 2 months worth of feed</returns>
    public async Task<Collection<Article>> GetMainFeedUpdate()
    {
        try
        {

            return await this.WebService.Get<Collection<Article>>(controller: "feeds",
                                                               action: "update",
                                                               parameters: [DateTime.Now.AddMonths(-2).ToString(_dateFormat)],
                                                               jsonBody: null,
                                                               unSuccessCallback: e => _ = HandleHttpException(e));
        }
#if DEBUG
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return null;
        }
#else
        catch
        {
            return null; 
        }
#endif
    }
    /// <summary>
    /// Get all the sources
    /// </summary>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public async Task<Collection<Source>> GetSources()
    {
        return Fetcher.Sources =  await WebService.Get<Collection<Source>>(controller: "sources", 
                                                        action: "getAll",
                                                        unSuccessCallback: e => _ = HandleHttpException(e));
    }
    /// <summary>
    /// Get the feed of an article
    /// </summary>
    /// <param name="keywords">keywords of the feed</param>
    /// <param name="timeUpdate">time of the last update (if applicable)</param>
    /// <param name="needUpdate">does the feed need an update</param>
    /// <returns></returns>
    public async Task<Collection<Article>> GetFeedArticles(string keywords, string timeUpdate = null, bool needUpdate = false)
    {
        try
        {
            if (string.IsNullOrEmpty(keywords))
                return new Collection<Article>();
            // Convert the spaces to make it url friendly
            keywords = keywords.Trim().Replace(' ', '+');

            return await WebService.Get<Collection<Article>>(controller: "feeds",
                                                             action: needUpdate ? "update" : null,
                                                             parameters: needUpdate ? [timeUpdate, keywords] : [keywords],
                                                             unSuccessCallback: (err) => _ = HandleHttpException(err));
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#else
            SentrySdk.CaptureException(ex);
#endif
            return new Collection<Article>();
        }
    }
    /// <summary>
    /// Get a brand new discord session from a resfresh token
    /// </summary>
    /// <param name="refreshToken">refreshToken given by discord auth</param>
    /// <returns></returns>
    public async Task<Session> RefreshDiscordSession(string refreshToken)
    {
        try
        {
            RefreshDiscordPayload payload = new()
            {
                RefreshToken = refreshToken
            };
            RefreshSessionResponse res = await WebService.Post<RefreshSessionResponse>(controller: "auth",
                                                                                        action: "discord/refresh_token",
                                                                                        jsonBody: JsonConvert.SerializeObject(payload),
                                                                                        unSuccessCallback: e => _ = HandleHttpException(e));

            if (res == null)
            {
                return null;
            }
            return res.Session;
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#else
            SentrySdk.CaptureException(ex);
#endif
            return null;
        }
    }
    /// <summary>
    /// Get the latest articles since given date
    /// </summary>
    /// <param name="dateUpdate">given date as "dd-MM-yyy_HH:mm:ss"</param>
    /// <returns>latest articles the date provided</returns>
    public async Task<Collection<Article>> GetMainFeedUpdate(string dateUpdate)
    {
        try
        {

            return await this.WebService.Get<Collection<Article>>(controller: "feeds",
                                                               action: "update",
                                                               parameters: new string[] { dateUpdate },
                                                               jsonBody: null,
                                                               unSuccessCallback: e => _ = HandleHttpException(e));
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#else
            SentrySdk.CaptureException(ex);
#endif
            return new Collection<Article>();
        }
    }
    /// <summary>
    /// Get the chunk articles from given date
    /// </summary>
    /// <param name="dateFrom">date from which the last article was published as "dd-MM-yyy_HH:mm:ss"</param>
    /// <param name="length">length of the chunk in hour</param>
    /// <returns>chunk articles the date provided</returns>
    public async Task<Collection<Article>> GetFeedChunk(DateTime dateFrom, int length)
    {
        try
        {
            //dateFrom = new DateTime(dateFrom.Year, dateFrom.Month, dateFrom.Day, dateFrom.Hour, dateFrom.Minute, 0);
            string[] parameters = new string[]
            {
               dateFrom.AddHours(-length).ToString("dd-MM-yyy_HH:mm:ss"),
               dateFrom.AddMinutes(-1).ToString("dd-MM-yyy_HH:mm:ss"),
            };
            return await this.WebService.Get<Collection<Article>>(controller: "feeds",
                                                               parameters: parameters,
                                                               jsonBody: null,
                                                               unSuccessCallback: e => _ = HandleHttpException(e));
        }
        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#else
            SentrySdk.CaptureException(ex);
#endif
            return null;
        }
    }

    /// <summary>
    /// Get all the partners
    /// </summary>
    /// <returns>partners</returns>
    public async Task<Collection<Partner>> GetPartners()
    {
        try
        {
            return await this.WebService.Get<Collection<Partner>>(controller: "partners",
                                                               unSuccessCallback: e => _ = HandleHttpException(e));
        }

        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#else
            SentrySdk.CaptureException(ex);
#endif
            return null;
        }
    }

    /// <summary>
    /// Get all the deals
    /// </summary>
    /// <returns>all the deals</returns>
    public async Task<Collection<Deal>> GetDeals()
    {
        try
        {
            return _deals = await this.WebService.Get<Collection<Deal>>(controller: "deals",
                                                               unSuccessCallback: e => _ = HandleHttpException(e));
        }

        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#else
            SentrySdk.CaptureException(ex);
#endif
            return null;
        }
    }

    /// <summary>
    /// Get an article
    /// </summary>
    /// <param name="articleId">id of the article we want</param>
    /// <returns>retrun the article</returns>
    public async Task<Article> GetArticle(string articleId)
    {
        try
        {
            return await this.WebService.Get<Article>(controller: "article",
                                                      parameters: new string[] { articleId },
                                                      unSuccessCallback: e => _ = HandleHttpException(e));
        }

        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#else
            SentrySdk.CaptureException(ex);
#endif
            return null;
        }
    }

    /// <summary>
    /// Register a notification entity
    /// </summary>
    /// <param name="token">notification token</param>
    /// <returns>retrun the article</returns>
    public async Task RegisterNotificationEntity(string token)
    {
        try
        {
            Dictionary<string, string> rqHeaders = new();
            if (UserData != null)
                rqHeaders.Add("Authorization", $"{await SecureStorage.GetAsync(nameof(Session.TokenType))} {await SecureStorage.GetAsync(nameof(Session.AccessToken))}");
#if DEBUG
            string res =
#endif
                await this.WebService.Post(controller: "monitor",
                                           action: "NE/create",
                                           parameters: new Dictionary<string, string>
                                           {
                                               { nameof(token), token }
                                           },
                                           singleUseHeaders: rqHeaders.Count > 0? rqHeaders: null,
                                           unSuccessCallback: e => _ = HandleHttpException(e));
#if DEBUG
            Debug.WriteLine($"NE/create: {res}");
#endif
        }

        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#else
            SentrySdk.CaptureException(ex);
#endif
        }
    }

    /// <summary>
    /// Get the status of a feed notification subscription
    /// </summary>
    /// <param name="feedID">ID of the feed concerned by the subscription</param>
    /// <param name="token">notification token link to the NE of the user</param>
    /// <returns>retrun the article</returns>
    public async Task<bool> CheckSubStatus(string feedID, string token)
    {
        try
        {
            Dictionary<string, string> rqHeaders = new();
            if (UserData != null)
                rqHeaders.Add("Authorization", $"{await SecureStorage.GetAsync(nameof(Session.TokenType))} {await SecureStorage.GetAsync(nameof(Session.AccessToken))}");

            var res = await this.WebService.Post<SubStatusRes>(controller: "monitor",
                                                               action: "NE/feedstatus",
                                                               parameters: new Dictionary<string, string>
                                                               {
                                                                   { nameof(token), token }
                                                               },
                                                               singleUseHeaders: rqHeaders.Count > 0? rqHeaders: null,
                                                               unSuccessCallback: e => _ = HandleHttpException(e));
            if (res is not null)
                return res.Enabled;
            return false;
        }

        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#else
            SentrySdk.CaptureException(ex);
#endif
            return false;
        }
    }

    /// <summary>
    /// Update a notification entity
    /// </summary>
    /// <param name="newToken">new notification token</param>
    /// <param name="oldToken">former notification token</param>
    /// <returns>retrun the article</returns>
    public async Task UpdateNotificationEntity(string newToken, string oldToken)
    {
        try
        {
            Dictionary<string, string> rqHeaders = new()
            {
                { "Authorization", $"{await SecureStorage.GetAsync(nameof(Session.TokenType))} {await SecureStorage.GetAsync(nameof(Session.AccessToken))}" }
            };
#if DEBUG
            string res =
#endif
                await this.WebService.Put(controller: "monitor",
                                           action: "NE/update",
                                           parameters: new Dictionary<string, string>
                                           {
                                               { nameof(oldToken), oldToken },
                                               { nameof(newToken), newToken }
                                           },
                                           singleUseHeaders: rqHeaders,
                                           unSuccessCallback: e => _ = HandleHttpException(e));
#if DEBUG
            Debug.WriteLine($"NE/update: {res}");
#endif
        }

        catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#else
            SentrySdk.CaptureException(ex);
#endif
        }
    }

    /// <summary>
    /// Save all the tokens of a session and expiration
    /// </summary>
    /// <param name="newSession"></param>
    public async Task SaveSession (Session newSession)
    {
        // Keep the current session
        CurrentSession = newSession;

        List<Task> tasks =
        [
            // Save sensitive data
            SecureStorage.SetAsync(nameof(Session.AccessToken), newSession.AccessToken),
            SecureStorage.SetAsync(nameof(Session.RefreshToken), newSession.RefreshToken),
            SecureStorage.SetAsync(nameof(Session.TokenType), newSession.TokenType),
        ];
        
        string token = await SecureStorage.GetAsync(AppConstant.NotificationToken);
        if (!string.IsNullOrEmpty(token))
            tasks.Add(UpdateNotificationEntity(token, token));

        await Task.WhenAll(tasks);

        // Save regular data about the session
        Preferences.Set(nameof(Session.ExpiresIn), newSession.ExpiresIn);
        Preferences.Set(nameof(Session.Created), newSession.Created);
    }

    /// <summary>
    /// Restore the last session if any
    /// </summary>
    public async Task RestoreSession ()
    {
        // Save regular data about the session
        int exp = Preferences.Get(nameof(Session.ExpiresIn), Int16.MinValue);
        string refreshToken = await SecureStorage.GetAsync(nameof(Session.RefreshToken)).ConfigureAwait(false);

        // If there was no session just leave
        if (string.IsNullOrEmpty(refreshToken)) return;

        DateTime dt = Preferences.Get(nameof(Session.Created), DateTime.MinValue);

        // Check if the token expired
        if ((DateTime.UtcNow - dt).TotalSeconds >= exp)
        {
            // Refresh the token
            await SaveSession(await RefreshDiscordSession(refreshToken));

            return;
        }

        if (!EvaluateCurrentSession()) return;

        // Save sensitive data
        var accessTask = SecureStorage.GetAsync(nameof(Session.AccessToken));
        var tokenTypeTask = SecureStorage.GetAsync(nameof(Session.TokenType));

        await Task.WhenAll(accessTask, tokenTypeTask);

        // Fill up the current session with the data gathered from the previous one
        CurrentSession = new()
        {
            AccessToken = accessTask.Result,
            RefreshToken = refreshToken,
            TokenType = tokenTypeTask.Result,
            ExpiresIn = exp
        };


    }

    /// <summary>
    /// Evaluate the validity and state of the current session
    /// </summary>
    /// <returns>true -> session alive; false -> dead session</returns>
    private bool EvaluateCurrentSession()
    {
        if (!RecoverUserInfo())
        {
            // Kill the previous session
            KillSession();
            return false;

        }
        return true;
    }

    /// <summary>
    /// Adding a hook to an article
    /// </summary>
    /// <param name="article">article hooked</param>
    public async Task RegisterHook(Article article)
    {
        var headers = new Dictionary<string, string>
        {
            { "x-api-key", AppConstant.MonitoringKey},
        };
        var paramss = new string[] { article.MongooseId };

       await WebService.Post(controller: "monitor",
                              action: "register",
                              singleUseHeaders: headers,
                              parameters: paramss,
                              unSuccessCallback: e => _ = HandleHttpException(e)
                               );
    }

    /// <summary>
    /// Kill a session
    /// </summary>
    public void KillSession()
    {
        // Empty the property
        CurrentSession = null;

        // Clear all data stored
        SecureStorage.Remove(nameof(Session.AccessToken));
        SecureStorage.Remove(nameof(Session.TokenType));


    }

    /// <summary>
    /// Recover the info relevant to the user
    /// </summary>
    /// <returns>true: data found | false: data not found</returns>
    public bool RecoverUserInfo()
    {

        // Get preferences
        string userDataStr = Preferences.Get(nameof(UserData), string.Empty);

        if (string.IsNullOrEmpty(userDataStr))
            return false;

        // Set Userdata object
        return (UserData = JsonConvert.DeserializeObject<User>(userDataStr)) != null;
    }

    /// <summary>
    /// Save the info relevant to the user
    /// </summary>
    public void SaveUserInfo(User user)
    {
        UserData = user;

        // Save preferences
        Preferences.Set(nameof(UserData), JsonConvert.SerializeObject(UserData));
    }

    /// <summary>
    /// Method to handle exceptions
    /// </summary>
    /// <param name="err"></param>
    /// <exception cref="Exception"></exception>
    private async Task HandleHttpException(HttpResponseMessage err)
    {
        string errMsg = await err.Content.ReadAsStringAsync();

        if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet
            && (errMsg.Contains("internet connection") || errMsg.Contains("Connection failure")))
            // If the error is being thrown because there is no internet: there is no point reporting it 
            return;
#if DEBUG
        throw new Exception(errMsg);
#else
        SentrySdk.CaptureException(new Exception(errMsg));
#endif
    }

    #region Local Actions

    /// <summary>
    /// Update a feed
    /// </summary>
    /// <param name="feed">Feed we want to update</param>
    /// <returns>Update status</returns>
    public async Task<int> UpdateFeed(Feed feed)
    {
        return await _generalDB.UpdateFeed(feed);
    }

    /// <summary>
    /// Delete a feed
    /// </summary>
    /// <param name="feed">Feed we want to delete</param>
    /// <returns>Update status</returns>
    public async Task<int> DeleteFeed(Feed feed)
    {
        return await _generalDB.DeleteFeed(feed);
    }

    /// <summary>
    /// Delete a article
    /// </summary>
    /// <param name="article">article we want to delete</param>
    /// <returns>Update status</returns>
    public async Task<int> DeleteArticle(Article article)
    {
        int res = -1;
        Article item = Bookmarks.SingleOrDefault(b => b.MongooseId == article.MongooseId);
        
        if (item is null)
            return res;

        res = await _generalDB.DeleteArticleBookmark(article);
        if (res == 1)
        {
            Bookmarks.RemoveAt(Bookmarks.IndexOf(item));
    }

        return res;
    }

    /// <summary>
    /// Check if an article exist
    /// </summary>
    /// <param name="articleId">Id of the article we want to check</param>
    /// <returns>true -> exist; false -> doesn't</returns>
    public bool ArticleExist (string articleId)
    {
        if (Bookmarks is null)
            return false;

        return Bookmarks.SingleOrDefault(a => a.MongooseId == articleId) is not null;
    }

    /// <summary>
    /// Load/Reload all the Bookmarks
    /// </summary>
    public async Task LoadBookmarks ()
    {
        
        Bookmarks = [.. await _generalDB.GetArticles()];
    }

    /// <summary>
    /// Load/Reload all the Bookmarks
    /// </summary>
    public async Task UpdateBackupSources ()
    {
        try
        {
            if (Fetcher.Sources is not null)
                await _backupDB.UpdateSources([.. Fetcher.Sources]);

        } catch (Exception ex)
        {
#if DEBUG
            Debug.WriteLine(ex);
#else
            SentrySdk.CaptureException(ex);
#endif

        }
    }

    /// <summary>
    /// Add a bookmark to an article
    /// </summary>
    /// <param name="article">article to be bookmarked</param>
    /// <returns>true -> exist; false -> doesn't</returns>
    public async Task<int> AddBookmark (Article article)
    {
        int res = -1;
        res =  await _generalDB.InsertArticleBookmark(article);
        if (res == 1)
            Bookmarks.Insert(0, article);
        return res;
    }

    /// <summary>
    /// Update deals
    /// </summary>
    /// <returns>count of new deals</returns>
    public async Task<int> UpdateDeals()
    {
        return await _generalDB.UpdateDeals(_deals);
    }

#endregion

}
