using System.Collections.ObjectModel;
using System.Windows.Input;
using Naidis_Mobilapp.Models;
using Naidis_Mobilapp.Services;

namespace Naidis_Mobilapp.ViewModels;

public class ExploreViewModel : BaseViewModel
{
    readonly CityDatabaseService databaseService;
    readonly List<CityPlace> allPlaces;
    CityCategory? selectedCategory;
    bool isCategoryMenuVisible;

    public ObservableCollection<CityCategory> Categories { get; } = new();

    public ObservableCollection<CityPlace> Places { get; } = new();

    public ICommand SelectCategoryCommand { get; }

    public ICommand ToggleCategoryMenuCommand { get; }

    public ExploreViewModel(CityDatabaseService databaseService)
    {
        this.databaseService = databaseService;
        allPlaces = CityDataService.GetPlaces();
        SelectCategoryCommand = new Command<CityCategory>(SelectCategory);
        ToggleCategoryMenuCommand = new Command(ToggleCategoryMenu);

        foreach (CityCategory category in CityDataService.GetCategories())
        {
            Categories.Add(category);
        }

        SelectCategory(Categories.First());
        LanguageService.LanguageChanged += RefreshLanguage;
    }

    public string PageTitle => Resources.Localization.AppResources.Get("CityExploreTitle");

    public string TapHint => Resources.Localization.AppResources.Get("CityTapHint");

    public string CategoryMenuButtonText => IsCategoryMenuVisible ? "\u00D7" : "\u2630";

    public CityCategory? SelectedCategory
    {
        get => selectedCategory;
        set => SetProperty(ref selectedCategory, value);
    }

    public bool IsCategoryMenuVisible
    {
        get => isCategoryMenuVisible;
        set
        {
            if (SetProperty(ref isCategoryMenuVisible, value))
            {
                OnPropertyChanged(nameof(CategoryMenuButtonText));
            }
        }
    }

    public async Task<bool> AddFavoriteAsync(CityPlace place)
    {
        if (await databaseService.IsFavoriteAsync(place.Id))
        {
            return false;
        }

        await databaseService.SaveFavoriteAsync(place);
        return true;
    }

    void SelectCategory(CityCategory? category)
    {
        if (category == null)
        {
            return;
        }

        SelectedCategory = category;
        Places.Clear();

        foreach (CityPlace place in allPlaces.Where(place => place.CategoryKey == category.Key))
        {
            Places.Add(place);
        }

        IsCategoryMenuVisible = false;
    }

    void ToggleCategoryMenu()
    {
        IsCategoryMenuVisible = !IsCategoryMenuVisible;
    }

    void RefreshLanguage()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(TapHint));
        OnPropertyChanged(nameof(CategoryMenuButtonText));

        foreach (CityCategory category in Categories.ToList())
        {
            int index = Categories.IndexOf(category);
            Categories[index] = category;
        }

        List<CityPlace> currentPlaces = Places.ToList();
        Places.Clear();
        foreach (CityPlace place in currentPlaces)
        {
            Places.Add(place);
        }
    }
}
