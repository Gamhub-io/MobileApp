using CommunityToolkit.Mvvm.Messaging.Messages;

namespace GamHubApp.Services.ChangedMessages;

public class GemsUpdatedMessage(int status = 0) : ValueChangedMessage<int>(status);
