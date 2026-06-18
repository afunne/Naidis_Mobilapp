namespace Naidis_Mobilapp.Models;

public class Recipe
{
    public string Name { get; set; } = "";

    public string Category { get; set; } = "";

    public string ImageLink { get; set; } = "";

    public string Description { get; set; } = "";

    public string Ingredients { get; set; } = "";

    public string Instructions { get; set; } = "";

    public string PrepTime { get; set; } = "";

    public string CookTime { get; set; } = "";

    public string Servings { get; set; } = "";

    public int Rating { get; set; }

    public string RatingText => Rating > 0 ? $"Hinne: {Rating}/5" : "Hindamata";
}
