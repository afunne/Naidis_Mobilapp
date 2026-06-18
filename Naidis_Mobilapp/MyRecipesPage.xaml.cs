using Naidis_Mobilapp.Models;
using Naidis_Mobilapp.Services;

namespace Naidis_Mobilapp;

public partial class MyRecipesPage : ContentPage
{
    List<Recipe> recipes = new();

    public MyRecipesPage()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadRecipes();
    }

    async void OnDeleteClicked(object sender, EventArgs e)
    {
        if (sender is not MenuItem menuItem || menuItem.CommandParameter is not Recipe recipe)
        {
            return;
        }

        bool delete = await DisplayAlertAsync("Kustuta retsept", $"Kas kustutada retsept \"{recipe.Name}\"?", "Kustuta", "Tühista");
        if (!delete)
        {
            return;
        }

        recipes.Remove(recipe);
        FileManager.SaveRecipes(recipes);
        ShowGroupedRecipes();
    }

    async void OnRecipeTapped(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is not Recipe recipe)
        {
            return;
        }

        RecipesList.SelectedItem = null;
        await Navigation.PushAsync(new RecipeDetailPage(recipe));
    }

    void LoadRecipes()
    {
        recipes = FileManager.ReadRecipes();
        ShowGroupedRecipes();
    }

    void ShowGroupedRecipes()
    {
        var groupedRecipes = recipes
            .OrderBy(recipe => recipe.Category)
            .ThenBy(recipe => recipe.Name)
            .GroupBy(recipe => recipe.Category)
            .Select(group => new RecipeCategory(group.Key, group))
            .ToList();

        RecipesList.ItemsSource = groupedRecipes;
    }
}
