
using CommunityToolkit.Maui;
using GamHubApp;
using GamHubApp.Core;

namespace Gamhub
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
#if Debug
            EnvironementSetup.DebugSetup();
#endif
            builder.UseMauiApp<App>()
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
                   }).UseMauiCommunityToolkit();
#if DEBUG
            //builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}