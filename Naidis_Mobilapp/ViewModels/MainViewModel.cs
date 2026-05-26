using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Naidis_Mobilapp.Resources.Localization;
using Naidis_Mobilapp.Services;

namespace Naidis_Mobilapp.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    public string Greeting => AppResources.GreetingText;
    public string ChangeLanguageLabel => AppResources.ChangeLanguage;
    public string EnglishButton => AppResources.EnglishButton;
    public string RussianButton => AppResources.RussianButton;
    public string EstonianButton => AppResources.EstonianButton;
    public string SavedLanguageText => AppResources.SavedLanguageText;

    public ICommand SetEnglishCommand { get; }
    public ICommand SetRussianCommand { get; }
    public ICommand SetEstonianCommand { get; }

    public MainViewModel()
    {
        SetEnglishCommand = new Command(() => LanguageService.ChangeLanguage("en"));
        SetRussianCommand = new Command(() => LanguageService.ChangeLanguage("ru"));
        SetEstonianCommand = new Command(() => LanguageService.ChangeLanguage("et"));

        LanguageService.LanguageChanged += OnLanguageChanged;
    }

    void OnLanguageChanged()
    {
        OnPropertyChanged(nameof(Greeting));
        OnPropertyChanged(nameof(ChangeLanguageLabel));
        OnPropertyChanged(nameof(EnglishButton));
        OnPropertyChanged(nameof(RussianButton));
        OnPropertyChanged(nameof(EstonianButton));
        OnPropertyChanged(nameof(SavedLanguageText));
    }

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
