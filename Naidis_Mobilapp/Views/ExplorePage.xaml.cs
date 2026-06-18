using Naidis_Mobilapp.Models;
using Naidis_Mobilapp.Resources.Localization;
using Naidis_Mobilapp.ViewModels;

namespace Naidis_Mobilapp.Views;

public partial class ExplorePage : ContentPage
{
    bool isAutoScrollActive;

    ExploreViewModel ViewModel => (ExploreViewModel)BindingContext;

    public ExplorePage(ExploreViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        isAutoScrollActive = true;
        StartAutoScroll();
    }

    protected override void OnDisappearing()
    {
        isAutoScrollActive = false;
        base.OnDisappearing();
    }

    async void OnPlaceTapped(object sender, TappedEventArgs e)
    {
        if (sender is not BindableObject bindable || bindable.BindingContext is not CityPlace place)
        {
            return;
        }

        bool addFavorite = await DisplayAlertAsync(
            place.Name,
            place.Detail,
            AppResources.Get("CityAddFavorite"),
            AppResources.Get("CityClose"));

        if (!addFavorite)
        {
            return;
        }

        bool added = await ViewModel.AddFavoriteAsync(place);
        string message = added
            ? AppResources.Get("CityFavoriteAdded")
            : AppResources.Get("CityFavoriteExists");

        await DisplayAlertAsync(AppResources.Get("CityFavoritesTitle"), message, AppResources.Get("OkButtonText"));
    }

    void StartAutoScroll()
    {
        Dispatcher.StartTimer(TimeSpan.FromSeconds(4), () =>
        {
            if (!isAutoScrollActive || ViewModel.Places.Count == 0)
            {
                return false;
            }

            PlacesCarousel.Position = (PlacesCarousel.Position + 1) % ViewModel.Places.Count;
            return true;
        });
    }
}
