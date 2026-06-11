using System.Globalization;
using System.Reflection;
using System.Resources;

namespace Naidis_Mobilapp.Resources.Localization;

public static class AppResources
{
    static readonly ResourceManager ResourceManager = new(
        "Naidis_Mobilapp.Resources.Localization.AppResources",
        typeof(AppResources).GetTypeInfo().Assembly);

    public static CultureInfo? Culture { get; set; }

    public static string GreetingText => GetString(nameof(GreetingText));
    public static string ChangeLanguage => GetString(nameof(ChangeLanguage));
    public static string EnglishButton => GetString(nameof(EnglishButton));
    public static string RussianButton => GetString(nameof(RussianButton));
    public static string EstonianButton => GetString(nameof(EstonianButton));
    public static string SavedLanguageText => GetString(nameof(SavedLanguageText));

    public static string Get(string name) => GetString(name);

    public static string GetString(string name)
    {
        return ResourceManager.GetString(name, Culture) ?? name;
    }
}
