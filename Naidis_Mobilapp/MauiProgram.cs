using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Naidis_Mobilapp.Services;
using Naidis_Mobilapp.ViewModels;
using Naidis_Mobilapp.Views;

namespace Naidis_Mobilapp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            SQLitePCL.Batteries_V2.Init();

            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit() // Register CommunityToolkit.Maui
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("BobloxClassic-nRjl4.ttf", "BobloxFont");
                });

            builder.Services.AddSingleton<CityDatabaseService>();
            builder.Services.AddTransient<ExploreViewModel>();
            builder.Services.AddTransient<FavoritesViewModel>();
            builder.Services.AddTransient<SettingsViewModel>();
            builder.Services.AddTransient<CityExplorerPage>();
            builder.Services.AddTransient<ExplorePage>();
            builder.Services.AddTransient<FavoritesPage>();
            builder.Services.AddTransient<SettingsPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
