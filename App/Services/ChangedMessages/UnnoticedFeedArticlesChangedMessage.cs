
using CommunityToolkit.Mvvm.Messaging.Messages;
using GamHubApp.Models;
using System.Collections.ObjectModel;

namespace GamHubApp.Services;

public class UnnoticedFeedArticlesChangedMessage : ValueChangedMessage<ObservableCollection<Article>>
{
    public UnnoticedFeedArticlesChangedMessage(ObservableCollection<Article> value) : base(value)
    {
        Count = value.Count;
    }

    public int Count { get; set; }
    public string Id { get; set; }
    public DateTime UpdateTime { get; private set; } = DateTime.Now;
}
