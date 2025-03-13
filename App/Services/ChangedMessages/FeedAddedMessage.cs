using CommunityToolkit.Mvvm.Messaging.Messages;
using GamHubApp.Models;

namespace GamHubApp.Services;

public class FeedAddedMessage : ValueChangedMessage<Feed>
{
    public FeedAddedMessage(Feed feed) : base(feed)
    {
    }
}
