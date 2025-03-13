using CommunityToolkit.Mvvm.Messaging.Messages;
using GamHubApp.ViewModels;
using GamHubApp.Views;

namespace GamHubApp.Services.ChangedMessages;

public class ScrollMainPageChangedMessage : ValueChangedMessage<BaseViewModel>
{
    public ScrollMainPageChangedMessage(BaseViewModel value) : base(value)
    {
    }
}
