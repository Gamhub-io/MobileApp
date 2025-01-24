using CommunityToolkit.Mvvm.Messaging.Messages;
using GamHubApp.Models;

namespace GamHubApp.Services;

public class BookmarkChangedMessage : ValueChangedMessage<Article>
{
    public Article ArticleSent { get; set; }
    public bool Saved { get; set; }
    public BookmarkChangedMessage(Article article, bool saved) : base(article)
    {
        ArticleSent = article;
        Saved = saved;
    }
}
