using CommunityToolkit.Mvvm.Messaging;
using GamHubApp.Services;
using Newtonsoft.Json;
using SQLite;
using System.Reflection;

namespace GamHubApp.Models;

public class Article
{
    public Article() 
    {
        Task.Run(async () =>
        {
            await Init();
        });

    }

    [PrimaryKey, Column("_id"), JsonProperty("uuid")]
    public string Id { get; set; }
    [Column("mongoId"), JsonProperty("_id")]
    public string MongooseId { get; set; }

    [JsonProperty("title")]
    public string Title { get; set; }
    [JsonProperty("textSnipet")]
    public string TextSnipet { get; set; }
    [JsonProperty("content")]
    public string Content { get; set; }
    [JsonProperty("author")]
    public string Author { get; set; }
    [JsonProperty("image")]
    public string Image { get; set; }[JsonProperty("sourceId")]
    public string SourceId { get; set; }
    [JsonProperty("blocked")]
    public bool? Blocked { get; set; }
    [JsonProperty("source"), Ignore]
    public Source Source { get; set; }
    [JsonProperty("isoDate")]
    public DateTime FullPublishDate { get; set; }
    [JsonProperty("categories"), Ignore]
    public string[] Categories { get; set; }
    public string PublishDate
    {
        get
        {
            return FullPublishDate.ToString("dd/MM/yyyy");
        }
    }
    public string PublishTime
    {
        get
        {
            return FullPublishDate.ToLocalTime().ToString("HH:mm");
        }
    }
    public string Url { get; set; }
    public TimeSpan Time
    {
        get
        {
            return DateTime.Now - this.FullPublishDate.ToLocalTime();
        }
    }
    private bool? _isSaved = null;
    public bool IsSaved
    {
        get
        {
            return (_isSaved ?? false);
        }
        set { _isSaved = value; }
    }
    /// <summary>
    /// Compare this article to another
    /// </summary>
    /// <param name="otherArticle">the other article you want to compare</param>
    /// <returns></returns>
    public bool IsEqualTo(Article otherArticle)
    {
        if (this == null || otherArticle == null)
        {
            return this == otherArticle;
        }
        foreach (var property in typeof(Article).GetProperties())
        {
            var value1 = property.GetValue(this);
            var value2 = property.GetValue(otherArticle);
            if (!Equals(value1, value2))
                return false;
            }
        return true;
    }

    public async Task Init()
    {
        if (_isSaved == null)
            IsSaved = await app.DataFetcher.ArticleExist(this.Id);
        if (app.DataFetcher.Sources is null)
            await app.DataFetcher.GetSources();

        if (Source is null)
        {
            Source = app.DataFetcher.Sources.SingleOrDefault(s => s.MongoId == SourceId);

        }

    }
    App app = (App.Current as App);

    [Ignore]
    public Command AddBookmark
    {
        get
        {
            return new Command(async() =>
            {
                // If the article is already in bookmarks
                bool isSaved = IsSaved;

                // Marked the article as saved
                IsSaved = !IsSaved;
                if (isSaved)
                    await app.DataFetcher.DeleteArticle(this);
                else
                    // Insert it in database
                    await app.DataFetcher.AddBookmark(this);

                // Say the the bookmark has been updated
                WeakReferenceMessenger.Default.Send(new BookmarkChangedMessage(this, IsSaved));
            }); ;
        }
    }

    [Ignore]
    public Command ShareArticle
    {
        get
        {
            return new Command(() =>
            {
                _ = Share.RequestAsync(new ShareTextRequest
                {
                    Uri = Url,
                    Title = "Share this article",
                    Subject = Title,
                    Text = Title
                });
            }); ;
        }
    }

    [Ignore]
    public string HTMLContent
    {
        get
        {

            var assembly = Assembly.GetExecutingAssembly();
            string style;
            // Read the embedded resource
            using (Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.Resources.Styles.ArticleStyle.css"))
            using (StreamReader reader = new (stream))
            {
                style = $"<style type=\"text/css\">{reader.ReadToEnd()}</style>";
            }
            return style+Content;
        }
    }
}