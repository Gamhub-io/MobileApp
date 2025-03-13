using CommunityToolkit.Mvvm.Messaging.Messages;
using GamHubApp.ViewModels;

namespace GamHubApp.Services;

public class ScrollFeedPageChangedMessage : ValueChangedMessage<BaseViewModel>
{
    public ScrollFeedPageChangedMessage(BaseViewModel value) : base(value)
    {
    }
}
