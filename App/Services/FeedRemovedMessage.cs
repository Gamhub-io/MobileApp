using CommunityToolkit.Mvvm.Messaging.Messages;
using GamHubApp.Models;

namespace GamHubApp.Services;

public class FeedRemovedMessage : ValueChangedMessage<Feed>
{
    public FeedRemovedMessage(Feed feed) : base(feed)
    {
    }
}
