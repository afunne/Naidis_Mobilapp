using Naidis_Mobilapp.Models;
using Naidis_Mobilapp.Services;

namespace Naidis_Mobilapp;

public partial class RecipeDetailPage : ContentPage
{
    readonly Recipe recipe;
    readonly string originalName;
    readonly string originalCategory;
    readonly string originalImageLink;
    static readonly List<string> MinuteOptions = Enumerable
        .Range(0, 37)
        .Select(index => $"{index * 5} min")
        .ToList();

    public RecipeDetailPage(Recipe recipe)
    {
        InitializeComponent();
        this.recipe = recipe;
        originalName = recipe.Name;
        originalCategory = recipe.Category;
        originalImageLink = recipe.ImageLink;
        BindingContext = recipe;
        UpdateInfoLabels();
        ShowAbout();
    }

    async void OnPinTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlertAsync("Kinnitatud", "Retsept on meeles hoitud.", "OK");
    }

    async void OnFavoriteTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlertAsync("Lemmik", "See retsept on sinu lemmikute hulgas.", "OK");
    }

    void OnCookTapped(object sender, TappedEventArgs e)
    {
        ShowInstructions();
    }

    async void OnPlanTapped(object sender, TappedEventArgs e)
    {
        await DisplayAlertAsync("Plaan", $"Retsept \"{recipe.Name}\" lisati tänasesse toiduplaani.", "OK");
    }

    void OnShopTapped(object sender, TappedEventArgs e)
    {
        ShowIngredients();
    }

    async void OnRateTapped(object sender, TappedEventArgs e)
    {
        string? rating = await DisplayActionSheetAsync("Vali hinnang", "Tühista", null, "1", "2", "3", "4", "5");
        if (!int.TryParse(rating, out int value))
        {
            return;
        }

        recipe.Rating = value;
        SaveCurrentRecipe();
        UpdateInfoLabels();
        await DisplayAlertAsync("Hinnatud", $"Andsid retseptile hinde {value}/5.", "OK");
    }

    async void OnEditTapped(object sender, TappedEventArgs e)
    {
        string? field = await DisplayActionSheetAsync(
            "Mida muuta?",
            "Tühista",
            null,
            "Kirjeldus",
            "Ettevalmistus",
            "Küpsetus",
            "Portsjonid");

        if (field == null || field == "Tühista")
        {
            return;
        }

        string currentValue = field switch
        {
            "Kirjeldus" => recipe.Description,
            "Ettevalmistus" => recipe.PrepTime,
            "Küpsetus" => recipe.CookTime,
            "Portsjonid" => recipe.Servings,
            _ => ""
        };

        string? newValue = await GetEditedValueAsync(field, currentValue);
        if (newValue == null)
        {
            return;
        }

        switch (field)
        {
            case "Kirjeldus":
                recipe.Description = newValue;
                break;
            case "Ettevalmistus":
                recipe.PrepTime = newValue;
                break;
            case "Küpsetus":
                recipe.CookTime = newValue;
                break;
            case "Portsjonid":
                recipe.Servings = newValue;
                break;
        }

        SaveCurrentRecipe();
        UpdateInfoLabels();
        RefreshBinding();
        ShowAbout();
    }

    async Task<string?> GetEditedValueAsync(string field, string currentValue)
    {
        if (field == "Ettevalmistus" || field == "Küpsetus")
        {
            string? selectedTime = await DisplayActionSheetAsync(
                $"Vali aeg ({field})",
                "Tühista",
                null,
                MinuteOptions.ToArray());

            return selectedTime == "Tühista" ? null : selectedTime;
        }

        return await DisplayPromptAsync("Muuda", field, initialValue: currentValue);
    }

    void OnAboutClicked(object sender, EventArgs e)
    {
        ShowAbout();
    }

    void OnIngredientsClicked(object sender, EventArgs e)
    {
        ShowIngredients();
    }

    void OnInstructionsClicked(object sender, EventArgs e)
    {
        ShowInstructions();
    }

    void ShowAbout()
    {
        SectionTitleLabel.Text = "Ülevaade";
        SectionTextLabel.Text = string.IsNullOrWhiteSpace(recipe.Description)
            ? "Kirjeldust ei ole lisatud."
            : recipe.Description;
    }

    void ShowIngredients()
    {
        SectionTitleLabel.Text = "Koostisosad";
        SectionTextLabel.Text = ToBulletedList(recipe.Ingredients, "Koostisosi ei ole lisatud.");
    }

    void ShowInstructions()
    {
        SectionTitleLabel.Text = "Valmistamisjuhend";
        SectionTextLabel.Text = ToNumberedList(recipe.Instructions, "Juhiseid ei ole lisatud.");
    }

    void UpdateInfoLabels()
    {
        ServingsLabel.Text = string.IsNullOrWhiteSpace(recipe.Servings) ? "Portsjonid: -" : $"Portsjonid: {recipe.Servings}";
        PrepLabel.Text = string.IsNullOrWhiteSpace(recipe.PrepTime) ? "Ettevalmistus: -" : $"Ettevalmistus: {recipe.PrepTime}";
        CookLabel.Text = string.IsNullOrWhiteSpace(recipe.CookTime) ? "Küpsetus: -" : $"Küpsetus: {recipe.CookTime}";
        RatingLabel.Text = recipe.Rating > 0 ? $"Hinne: {recipe.Rating}/5" : "Hindamata";
        RatingActionLabel.Text = recipe.Rating > 0 ? recipe.Rating.ToString() : "-";
    }

    void SaveCurrentRecipe()
    {
        FileManager.SaveRecipeChanges(originalName, originalCategory, originalImageLink, recipe);
    }

    void RefreshBinding()
    {
        BindingContext = null;
        BindingContext = recipe;
    }

    static string ToBulletedList(string text, string fallback)
    {
        string[] lines = GetLines(text);
        return lines.Length == 0
            ? fallback
            : string.Join(Environment.NewLine, lines.Select(line => $"- {line}"));
    }

    static string ToNumberedList(string text, string fallback)
    {
        string[] lines = GetLines(text);
        return lines.Length == 0
            ? fallback
            : string.Join(Environment.NewLine, lines.Select((line, index) => $"{index + 1}. {line}"));
    }

    static string[] GetLines(string text)
    {
        return text
            .Split(new[] { "\r\n", "\n" }, StringSplitOptions.None)
            .Select(line => line.Trim())
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();
    }
}
