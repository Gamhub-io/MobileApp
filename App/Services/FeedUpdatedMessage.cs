using CommunityToolkit.Mvvm.Messaging.Messages;
using GamHubApp.Models;

namespace GamHubApp.Services;

public class FeedUpdatedMessage : ValueChangedMessage<Feed>
{
    public FeedUpdatedMessage(Feed feed) : base(feed)
    {
    }
}
