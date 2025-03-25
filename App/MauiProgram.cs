using CommunityToolkit.Maui;
using Vapolia.StrokedLabels;
using GamHubApp.Services;
using GamHubApp.ViewModels;
using GamHubApp.Views;

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
                   fonts.AddFont("Lexend-SemiBold.ttf", "P-SemiBold");
               })
               .RegisterViews()
               .RegisterViewModels()
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
        builder.Services.AddSingleton<Fetcher>();
        builder.Services.AddSingleton<GeneralDataBase>();

        builder.Services.AddSingleton<CommunityToolkit.Maui.Behaviors.TouchBehavior>();
        return builder.Build();
    }

    public static MauiAppBuilder RegisterViewModels(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<AboutViewModel>();
        mauiAppBuilder.Services.AddSingleton<AppShellViewModel>();
        mauiAppBuilder.Services.AddSingleton<BookmarkViewModel>();
        mauiAppBuilder.Services.AddSingleton<FeedsViewModel>();
        mauiAppBuilder.Services.AddSingleton<NewsViewModel>();

        return mauiAppBuilder;
    }

    public static MauiAppBuilder RegisterViews(this MauiAppBuilder mauiAppBuilder)
    {
        mauiAppBuilder.Services.AddSingleton<App>();
        mauiAppBuilder.Services.AddSingleton<AppShell>();

        // pages
        mauiAppBuilder.Services.AddSingleton<AboutPage>();
        mauiAppBuilder.Services.AddSingleton<FeedsPage>();
        mauiAppBuilder.Services.AddSingleton<BookmarkPage>();
        mauiAppBuilder.Services.AddSingleton<DeleteFeedPopUp>();
        mauiAppBuilder.Services.AddSingleton<RenameFeedPopUp>();
        return mauiAppBuilder;
    }
}