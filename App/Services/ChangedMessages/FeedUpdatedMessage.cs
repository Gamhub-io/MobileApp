using CommunityToolkit.Mvvm.Messaging.Messages;
using GamHubApp.Models;

namespace GamHubApp.Services;

public class FeedUpdatedMessage : ValueChangedMessage<Feed>
{
    public Feed Feed { get; set; }
    public FeedUpdatedMessage(Feed feed) : base(feed)
    {
        Feed = feed;
    }
}
