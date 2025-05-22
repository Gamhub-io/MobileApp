using Android.App;
using Plugin.FirebasePushNotifications.Platforms.Channels;

namespace GamHubApp.Platforms.Android.Notifications;

public static class NotificationChannelGamHub
{
    public static NotificationChannelRequest Default { get; } = new NotificationChannelRequest
    {
        ChannelId = "default_channel_id",
        ChannelName = "General",
        Description = "The default notification channel",
        LockscreenVisibility = NotificationVisibility.Public,
        IsDefault = true,
    };
    public static IEnumerable<NotificationChannelRequest> GetAll()
    {
        yield return Default;

        yield return new NotificationChannelRequest
        {
            ChannelId = "daily_catchup",
            ChannelName = "Daily catch up",
            Description = "Notifying you of the biggest news of the day",
            LockscreenVisibility = NotificationVisibility.Public,
#pragma warning disable CA1416 // Validate platform compatibility
            Importance = OperatingSystem.IsAndroidVersionAtLeast(24)? NotificationImportance.High : NotificationImportance.Default,
#pragma warning restore CA1416 // Validate platform compatibility
        };

        yield return new NotificationChannelRequest
        {
            ChannelId = "feed_subscription",
            ChannelName = "Your feeds",
            Description = "Notifying you of news articles comming up in your feed",
            LockscreenVisibility = NotificationVisibility.Public,
#pragma warning disable CA1416 // Validate platform compatibility
            Importance = OperatingSystem.IsAndroidVersionAtLeast(24)? NotificationImportance.High : NotificationImportance.Default,
#pragma warning restore CA1416 // Validate platform compatibility
        };

        // TODO: add more in the future
    }
}
