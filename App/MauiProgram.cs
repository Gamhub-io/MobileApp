using CommunityToolkit.Maui;
#if ANDROID
using GamHubApp.Platforms.Android.Renderers;
#endif

namespace GamHubApp;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
#if Debug
        EnvironementSetup.DebugSetup();
#endif
        builder.UseMauiApp<App>()
            .UseSentry(options => {
                // The DSN is the only required setting.
                options.Dsn = "https://6a618edd553c62a09273c0899febf030@o4508638278844416.ingest.de.sentry.io/4508638282580048";

                // Use debug mode if you want to see what the SDK is doing.
                // Debug messages are written to stdout with Console.Writeline,
                // and are viewable in your IDE's debug console or with 'adb logcat', etc.
                // This option is not recommended when deploying your application.
#if DEBUG
                options.Debug = true;
#else
                options.Debug = false;
#endif
    
                // Set TracesSampleRate to 1.0 to capture 100% of transactions for tracing.
                // We recommend adjusting this value in production.
                options.TracesSampleRate = 1.0;

                // Other Sentry options can be set here.
            })
               .ConfigureFonts(fonts =>
               {
                   fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                   fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                   fonts.AddFont("ComicShark.otf", "ComicShark");
                   fonts.AddFont("SonicComics.ttf", "SonicComics");
                   fonts.AddFont("MouseMemoirs-Regular.ttf", "MouseMemoirs-Regular");
                   fonts.AddFont("FontAwesome6Free-Regular-400.otf", "FaRegular");
                   fonts.AddFont("FontAwesome6Brands-Regular-400.otf", "FaBrand");
                   fonts.AddFont("FontAwesome6Free-Solid-900.otf", "FaSolid");
                   fonts.AddFont("Ubuntu-Regular.ttf", "P-Regular");
                   fonts.AddFont("Ubuntu-Bold.ttf", "P-Bold");
                   fonts.AddFont("Ubuntu-Medium.ttf", "P-Medium");
               })
               .UseMauiCommunityToolkit()
               .ConfigureMauiHandlers(handlers =>
               {
#if ANDROID
                   handlers.AddHandler(typeof(Shell), typeof(AndroidShellRenderer));
#endif
               });
#if DEBUG
        //builder.Logging.AddDebug();
#endif
        return builder.Build();
    }
}