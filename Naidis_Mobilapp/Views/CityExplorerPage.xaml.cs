using Naidis_Mobilapp.Services;
using Naidis_Mobilapp.ViewModels;

namespace Naidis_Mobilapp.Views;

public partial class CityExplorerPage : TabbedPage
{
    public CityExplorerPage()
    {
        InitializeComponent();

        var databaseService = new CityDatabaseService();

        Children.Add(new ExplorePage(new ExploreViewModel(databaseService)));
        Children.Add(new FavoritesPage(new FavoritesViewModel(databaseService)));
        Children.Add(new SettingsPage(new SettingsViewModel()));
    }
}
