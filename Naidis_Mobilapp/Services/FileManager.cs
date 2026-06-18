using Naidis_Mobilapp.Models;

namespace Naidis_Mobilapp.Services;

public static class FileManager
{
    static string FilePath => Path.Combine(FileSystem.AppDataDirectory, "recipes.txt");

    public static void AddRecipe(Recipe recipe)
    {
        Directory.CreateDirectory(FileSystem.AppDataDirectory);
        File.AppendAllText(FilePath, ToFileLine(recipe) + Environment.NewLine);
    }

    public static void SaveRecipes(IEnumerable<Recipe> recipes)
    {
        Directory.CreateDirectory(FileSystem.AppDataDirectory);
        File.WriteAllLines(FilePath, recipes.Select(ToFileLine));
    }

    public static List<Recipe> ReadRecipes()
    {
        var list = new List<Recipe>();

        if (!File.Exists(FilePath))
        {
            list = GetDefaultRecipes();
            SaveRecipes(list);
            return list;
        }

        string[] read = File.ReadAllLines(FilePath);
        foreach (string line in read)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(';');
                if (parts.Length >= 3)
                {
                    list.Add(FillRecipeDetails(new Recipe
                    {
                        Name = Restore(parts[0]),
                        Category = Restore(parts[1]),
                        ImageLink = Restore(parts[2]),
                        Description = parts.Length > 3 ? Restore(parts[3]) : "",
                        Ingredients = parts.Length > 4 ? Restore(parts[4]) : "",
                        Instructions = parts.Length > 5 ? Restore(parts[5]) : "",
                        PrepTime = parts.Length > 6 ? Restore(parts[6]) : "",
                        CookTime = parts.Length > 7 ? Restore(parts[7]) : "",
                        Servings = parts.Length > 8 ? Restore(parts[8]) : "",
                        Rating = parts.Length > 9 && int.TryParse(parts[9], out int rating) ? rating : 0
                    }));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        SaveRecipes(list);
        return list;
    }

    public static void SaveRecipeChanges(string originalName, string originalCategory, string originalImageLink, Recipe changedRecipe)
    {
        List<Recipe> recipes = ReadRecipes();
        int index = recipes.FindIndex(recipe =>
            recipe.Name == originalName &&
            recipe.Category == originalCategory &&
            recipe.ImageLink == originalImageLink);

        if (index >= 0)
        {
            recipes[index] = changedRecipe;
        }
        else
        {
            recipes.Add(changedRecipe);
        }

        SaveRecipes(recipes);
    }

    static string ToFileLine(Recipe recipe)
    {
        return string.Join(";",
            Clean(recipe.Name),
            Clean(recipe.Category),
            Clean(recipe.ImageLink),
            Clean(recipe.Description),
            Clean(recipe.Ingredients),
            Clean(recipe.Instructions),
            Clean(recipe.PrepTime),
            Clean(recipe.CookTime),
            Clean(recipe.Servings),
            recipe.Rating.ToString());
    }

    static string Clean(string value)
    {
        return value.Trim()
            .Replace(";", ",")
            .Replace("\r\n", "\\n")
            .Replace("\n", "\\n");
    }

    static string Restore(string value)
    {
        return value.Replace("\\n", Environment.NewLine);
    }

    static List<Recipe> GetDefaultRecipes()
    {
        return new List<Recipe>
        {
            new Recipe
            {
                Name = "Margarita pitsa",
                Category = "Põhiroad",
                ImageLink = "https://upload.wikimedia.org/wikipedia/commons/3/3a/Margherita_PIzza_%28Unsplash%29.jpg",
                Rating = 5
            },
            new Recipe
            {
                Name = "Kana shawarma",
                Category = "Põhiroad",
                ImageLink = "https://upload.wikimedia.org/wikipedia/commons/5/5f/Shawarma-sandwich-01.jpg",
                Rating = 4
            },
            new Recipe
            {
                Name = "Šokolaadikook",
                Category = "Magustoidud",
                ImageLink = "https://upload.wikimedia.org/wikipedia/commons/d/d8/Chocolate_Cake_%28Unsplash%29.jpg",
                Rating = 5
            }
        }.Select(FillRecipeDetails).ToList();
    }

    static Recipe FillRecipeDetails(Recipe recipe)
    {
        if (!string.IsNullOrWhiteSpace(recipe.Instructions))
        {
            return recipe;
        }

        string name = recipe.Name.ToLowerInvariant();
        if (name.Contains("pitsa"))
        {
            recipe.Description = "Õhuke ja krõbe Margarita pitsa tomatikastme, mozzarella ja basiilikuga.";
            recipe.Ingredients = "Pitsapõhi\nTomatikaste\nMozzarella\nVärske basiilik\nOliiviõli\nSool ja pipar";
            recipe.Instructions = "Määri pitsapõhjale tomatikaste.\nLisa mozzarella ja maitsesta.\nKüpseta 220 °C juures 10-12 minutit.\nLisa värske basiilik ja serveeri kuumalt.";
            recipe.PrepTime = "15 min";
            recipe.CookTime = "12 min";
            recipe.Servings = "2";
        }
        else if (name.Contains("shawarma"))
        {
            recipe.Description = "Mahlane kana shawarma värske salati ja kastmega pehmes saias või lavašis.";
            recipe.Ingredients = "Kanafilee\nLavaš või pita\nSalat\nTomat\nKurk\nJogurtikaste\nShawarma maitseaine";
            recipe.Instructions = "Maitsesta kana ja prae kuldseks.\nLõika köögiviljad ribadeks.\nSoojenda lavaš või pita.\nLisa kana, köögiviljad ja kaste.\nKeera kokku ja serveeri.";
            recipe.PrepTime = "20 min";
            recipe.CookTime = "15 min";
            recipe.Servings = "2";
        }
        else if (name.Contains("kook") || name.Contains("šokolaad"))
        {
            recipe.Description = "Lihtne šokolaadikook pehme sisu ja rikkaliku maitsega.";
            recipe.Ingredients = "Jahu\nKakao\nSuhkur\nMunad\nVõi\nTume šokolaad\nKüpsetuspulber";
            recipe.Instructions = "Sulata või ja šokolaad.\nSega kuivained eraldi kausis.\nLisa munad ja šokolaadisegu.\nVala tainas vormi.\nKüpseta 180 °C juures 25-30 minutit.";
            recipe.PrepTime = "15 min";
            recipe.CookTime = "30 min";
            recipe.Servings = "6";
        }

        return recipe;
    }
}
