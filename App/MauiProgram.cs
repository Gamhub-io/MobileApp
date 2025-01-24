using CommunityToolkit.Maui;
using Vapolia.StrokedLabels;
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
               
#if DEBUG
            // Sentry just pollute my logs in Debug so I disabled it
            options.Dsn = "";
#else 
            // The DSN is the only required setting.
            options.Dsn = "https://6a618edd553c62a09273c0899febf030@o4508638278844416.ingest.de.sentry.io/4508638282580048";
#endif

#if DEBUG
                options.Debug = true;
#else
                options.Debug = false;
#endif
                options.TracesSampleRate = 1.0;
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
               .UseStrokedLabelBehavior()
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