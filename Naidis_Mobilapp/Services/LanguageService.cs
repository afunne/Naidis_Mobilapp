using System.Globalization;
using Naidis_Mobilapp.Resources.Localization;

namespace Naidis_Mobilapp.Services;

public static class LanguageService
{
    const string LanguagePreferenceKey = "AppLanguage";

    public static event Action? LanguageChanged;

    public static void ApplySavedLanguage()
    {
        string savedLanguage = Preferences.Get(LanguagePreferenceKey, "en");
        ChangeLanguage(savedLanguage, save: false);
    }

    public static void ChangeLanguage(string languageCode)
    {
        ChangeLanguage(languageCode, save: true);
    }

    static void ChangeLanguage(string languageCode, bool save)
    {
        var culture = new CultureInfo(languageCode);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
        Thread.CurrentThread.CurrentCulture = culture;
        Thread.CurrentThread.CurrentUICulture = culture;

        AppResources.Culture = culture;

        if (save)
        {
            Preferences.Set(LanguagePreferenceKey, languageCode);
        }

        LanguageChanged?.Invoke();
    }
}
