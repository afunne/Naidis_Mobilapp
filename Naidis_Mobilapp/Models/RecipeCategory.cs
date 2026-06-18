namespace Naidis_Mobilapp.Models;

public class RecipeCategory : List<Recipe>
{
    public RecipeCategory(string name, IEnumerable<Recipe> recipes) : base(recipes)
    {
        Name = name;
    }

    public string Name { get; set; }
}
