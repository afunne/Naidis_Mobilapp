using System.Windows.Input;
using Naidis_Mobilapp.Services;

namespace Naidis_Mobilapp.ViewModels;

public class SettingsViewModel : BaseViewModel
{
    public SettingsViewModel()
    {
        ChangeLanguageCommand = new Command<string>(LanguageService.ChangeLanguage);
        LanguageService.LanguageChanged += RefreshLanguage;
    }

    public string PageTitle => Resources.Localization.AppResources.Get("CitySettingsTitle");

    public string LanguageTitle => Resources.Localization.AppResources.Get("CityLanguageTitle");

    public string SavedText => Resources.Localization.AppResources.Get("CityLanguageSaved");

    public string EstonianText => Resources.Localization.AppResources.Get("EstonianButton");

    public string EnglishText => Resources.Localization.AppResources.Get("EnglishButton");

    public string RussianText => Resources.Localization.AppResources.Get("RussianButton");

    public ICommand ChangeLanguageCommand { get; }

    void RefreshLanguage()
    {
        OnPropertyChanged(nameof(PageTitle));
        OnPropertyChanged(nameof(LanguageTitle));
        OnPropertyChanged(nameof(SavedText));
        OnPropertyChanged(nameof(EstonianText));
        OnPropertyChanged(nameof(EnglishText));
        OnPropertyChanged(nameof(RussianText));
    }
}
