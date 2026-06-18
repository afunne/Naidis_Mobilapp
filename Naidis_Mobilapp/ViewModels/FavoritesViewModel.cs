using System.Collections.ObjectModel;
using Naidis_Mobilapp.Models;
using Naidis_Mobilapp.Services;

namespace Naidis_Mobilapp.ViewModels;

public class FavoritesViewModel : BaseViewModel
{
    readonly CityDatabaseService databaseService;
    bool isEmpty;

    public ObservableCollection<CityPlace> Favorites { get; } = new();

    public FavoritesViewModel(CityDatabaseService databaseService)
    {
        this.databaseService = databaseService;
        LanguageService.LanguageChanged += RefreshLanguage;
    }

    public string PageTitle => Resources.Localization.AppResources.Get("CityFavoritesTitle");

    public string EmptyText => Resources.Localization.AppResources.Get("CityFavoritesEmpty");

    public bool IsEmpty
    {
        get => isEmpty;
        set => SetProperty(ref isEmpty, value);
    }

    public async Task LoadAsync()
    {
        Favorites.Clear();
        List<CityPlace> favorites = await databaseService.GetFavoritesAsync();

        foreach (CityPlace place in favorites)
        {
            Favorites.Add(place);
        }

        IsEmpty = Favorites.Count == 0;
    }

    public async Task DeleteAsync(CityPlace place)
    {
        await databaseService.DeleteFavoriteAsync(place);
        Favorites.Remove(place);
        IsEmpty = Favorites.Count == 0;
    }

    void RefreshLanguage()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(EmptyText));

        List<CityPlace> places = Favorites.ToList();
        Favorites.Clear();
        foreach (CityPlace place in places)
        {
            Favorites.Add(place);
        }
    }
}
