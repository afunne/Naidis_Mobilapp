using Naidis_Mobilapp.Models;
using Naidis_Mobilapp.ViewModels;

namespace Naidis_Mobilapp.Views;

public partial class FavoritesPage : ContentPage
{
    FavoritesViewModel ViewModel => (FavoritesViewModel)BindingContext;

    public FavoritesPage(FavoritesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await ViewModel.LoadAsync();
    }

    async void OnDeleteInvoked(object sender, EventArgs e)
    {
        if (sender is not SwipeItem swipeItem || swipeItem.BindingContext is not CityPlace place)
        {
            return;
        }

        await ViewModel.DeleteAsync(place);
    }
}
