using CommunityToolkit.Maui;
namespace GamHubApp;

public static class MauiProgramExtensions
{
    public static MauiAppBuilder UseSharedMauiApp(this MauiAppBuilder builder)
    {
        builder
            .UseMauiCommunityToolkit()
            .UseMauiApp<App>().ConfigureFonts(fonts =>
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
            });

        // TODO: Add the entry points to your Apps here.
        // See also: https://learn.microsoft.com/dotnet/maui/fundamentals/app-lifecycle
        //builder.Services.AddTransient<AppShell, AppShell>();


        return builder;
    }
}
