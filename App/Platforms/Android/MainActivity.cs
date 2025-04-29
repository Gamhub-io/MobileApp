using Android.App;
using Android.Content.PM;
using Android.OS;
using GamHubApp.Platforms.Android;

namespace GamHubApp;

[Activity(Theme = "@style/Maui.SplashTheme", LaunchMode = Android.Content.PM.LaunchMode.SingleTask, MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
public class MainActivity : MauiAppCompatActivity
{
    protected override void OnCreate(Bundle savedInstanceState)
    {
        base.OnCreate(savedInstanceState);

        OnBackPressedDispatcher.AddCallback(this, new BackPress());
    }
}