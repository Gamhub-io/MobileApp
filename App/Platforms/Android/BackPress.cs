using AndroidX.Activity;
using Microsoft.Maui.LifecycleEvents;

namespace GamHubApp.Platforms.Android;

internal class BackPress : OnBackPressedCallback
{
    public BackPress() : base(true)
    {

    }

    public override void HandleOnBackPressed()
    {
        if (Platform.CurrentActivity is not null
            && IPlatformApplication.Current?.Services is not null)
        {
            InvokeLifecycleEvents<AndroidLifecycle.OnBackPressed>(IPlatformApplication.Current.Services, del =>
            {
                del(Platform.CurrentActivity);
            });
        }
    }

    internal static void InvokeLifecycleEvents<TDelegate>(IServiceProvider services, Action<TDelegate> action)
        where TDelegate : Delegate
    {
        if (services == null)
        {
            return;
        }

        var delegates = GetLifecycleEventDelegates<TDelegate>(services);

        foreach (var del in delegates)
        {
            action?.Invoke(del);
        }
    }

    internal static IEnumerable<TDelegate> GetLifecycleEventDelegates<TDelegate>(IServiceProvider? services, string? eventName = null)
        where TDelegate : Delegate
    {
        var lifecycleService = services?.GetService<ILifecycleEventService>();

        if (lifecycleService == null)
        {
            yield break;
        }

        eventName ??= typeof(TDelegate).Name;

        foreach (var del in lifecycleService.GetEventDelegates<TDelegate>(eventName))
        {
            yield return del;
        }
    }
}
