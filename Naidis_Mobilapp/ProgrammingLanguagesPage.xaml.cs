using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Naidis_Mobilapp.Resources.Localization;
using Naidis_Mobilapp.Services;

namespace Naidis_Mobilapp;

public partial class ProgrammingLanguagesPage : ContentPage
{
    public sealed class LanguageCard : INotifyPropertyChanged
    {
        readonly string titleKey;
        readonly string descriptionKey;
        readonly string detailKey;

        public LanguageCard(string imageName, Color accentColor, string titleKey, string descriptionKey, string detailKey)
        {
            ImageName = imageName;
            AccentColor = accentColor;
            this.titleKey = titleKey;
            this.descriptionKey = descriptionKey;
            this.detailKey = detailKey;
        }

        public string ImageName { get; }

        public Color AccentColor { get; }

        public event PropertyChangedEventHandler? PropertyChanged;

        public string Title => AppResources.Get(titleKey);

        public string Description => AppResources.Get(descriptionKey);

        public string DetailText => AppResources.Get(detailKey);

        public string TapHint => AppResources.Get("TapHintText");

        public void RefreshLocalizedText()
        {
            OnPropertyChanged(nameof(Title));
            OnPropertyChanged(nameof(Description));
            OnPropertyChanged(nameof(DetailText));
            OnPropertyChanged(nameof(TapHint));
        }

        void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public ObservableCollection<LanguageCard> Items { get; } = new();

    bool autoScrollEnabled;
    bool languageSubscribed;
    int position;

    public ProgrammingLanguagesPage()
    {
        InitializeComponent();

        Title = AppResources.Get("KarussellPageTitle");

        Items.Add(new LanguageCard(
            "c_sharp_logo_2023_png.png",
            Color.FromArgb("#512BD4"),
            "CSharpTitle",
            "CSharpDescription",
            "CSharpAlertMessage"));

        Items.Add(new LanguageCard(
            "python_logo_notext_png.png",
            Color.FromArgb("#3776AB"),
            "PythonTitle",
            "PythonDescription",
            "PythonAlertMessage"));

        Items.Add(new LanguageCard(
            "logo_of_tc39_png.png",
            Color.FromArgb("#F7DF1E"),
            "JavaScriptTitle",
            "JavaScriptDescription",
            "JavaScriptAlertMessage"));

        Items.Add(new LanguageCard(
            "java_programming_language_logo_png.png",
            Color.FromArgb("#ED8B00"),
            "JavaTitle",
            "JavaDescription",
            "JavaAlertMessage"));

        Items.Add(new LanguageCard(
            "iso_cpp_logo_png.png",
            Color.FromArgb("#00599C"),
            "CppTitle",
            "CppDescription",
            "CppAlertMessage"));

        LanguageCarousel.ItemsSource = Items;
        LanguageCarousel.IndicatorView = CarouselIndicators;

        UpdateLocalizedText();
        autoScrollEnabled = false;
        StartAutoScroll();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        autoScrollEnabled = true;

        if (!languageSubscribed)
        {
            LanguageService.LanguageChanged += OnLanguageChanged;
            languageSubscribed = true;
        }

        UpdateLocalizedText();
    }

    protected override void OnDisappearing()
    {
        autoScrollEnabled = false;

        if (languageSubscribed)
        {
            LanguageService.LanguageChanged -= OnLanguageChanged;
            languageSubscribed = false;
        }

        base.OnDisappearing();
    }

    void UpdateLocalizedText()
    {
        Title = AppResources.Get("KarussellPageTitle");
        HeaderTitleLabel.Text = AppResources.Get("KarussellPageTitle");
        HeaderSubtitleLabel.Text = AppResources.Get("KarussellPageSubtitle");
        LanguageLabel.Text = AppResources.Get("LanguageSelectorLabel");
        InstructionsLabel.Text = AppResources.Get("InstructionText");

        EnglishLanguageButton.Text = AppResources.Get("EnglishButton");
        EstonianLanguageButton.Text = AppResources.Get("EstonianButton");
        AnimateButton.Text = AppResources.Get("AnimateButtonText");
        RestartButton.Text = AppResources.Get("RestartButtonText");

        foreach (LanguageCard item in Items)
        {
            item.RefreshLocalizedText();
        }

        UpdateLanguageButtons();
        UpdateCurrentLanguageLabel();
        UpdateCounter();
    }

    void UpdateLanguageButtons()
    {
        string currentLanguage = AppResources.Culture?.TwoLetterISOLanguageName ?? "en";
        bool estonianSelected = currentLanguage == "et";
        bool englishSelected = !estonianSelected;

        EnglishLanguageButton.BackgroundColor = englishSelected
            ? Color.FromArgb("#F9D923")
            : Color.FromArgb("#2F3C52");
        EnglishLanguageButton.TextColor = englishSelected
            ? Color.FromArgb("#10233E")
            : Colors.White;

        EstonianLanguageButton.BackgroundColor = estonianSelected
            ? Color.FromArgb("#2F8F9D")
            : Color.FromArgb("#2F3C52");
        EstonianLanguageButton.TextColor = Colors.White;

        EnglishLanguageButton.Opacity = englishSelected ? 1 : 0.88;
        EstonianLanguageButton.Opacity = estonianSelected ? 1 : 0.88;
    }

    void UpdateCurrentLanguageLabel()
    {
        string currentLanguage = AppResources.Culture?.TwoLetterISOLanguageName ?? "en";
        string languageName = currentLanguage == "et"
            ? AppResources.Get("CurrentLanguageEstonian")
            : AppResources.Get("CurrentLanguageEnglish");

        CurrentLanguageLabel.Text = string.Format(AppResources.Get("CurrentLanguageLabel"), languageName);
    }

    void UpdateCounter()
    {
        CounterLabel.Text = string.Format(
            AppResources.Get("CardCounterFormat"),
            Math.Min(position + 1, Items.Count),
            Items.Count);
    }

    void StartAutoScroll()
    {
        Dispatcher.StartTimer(TimeSpan.FromSeconds(4), () =>
        {
            if (!autoScrollEnabled || Items.Count < 2)
            {
                return true;
            }

            position = (position + 1) % Items.Count;
            LanguageCarousel.Position = position;
            return true;
        });
    }

    void OnLanguageChanged()
    {
        UpdateLocalizedText();
    }

    void OnPositionChanged(object? sender, PositionChangedEventArgs e)
    {
        position = e.CurrentPosition;
        UpdateCounter();
    }

    async void OnCardTapped(object? sender, TappedEventArgs e)
    {
        if ((sender as BindableObject)?.BindingContext is not LanguageCard item)
        {
            return;
        }

        await DisplayAlertAsync(item.Title, item.DetailText, AppResources.Get("OkButtonText"));
    }

    async void OnAnimateClicked(object sender, EventArgs e)
    {
        await LanguageCarousel.FadeToAsync(0.35, 120);
        await LanguageCarousel.FadeToAsync(1, 180);
    }

    void OnRestartClicked(object sender, EventArgs e)
    {
        position = 0;
        LanguageCarousel.Position = 0;
        UpdateCounter();
    }

    void OnEnglishClicked(object sender, EventArgs e)
    {
        LanguageService.ChangeLanguage("en");
    }

    void OnEstonianClicked(object sender, EventArgs e)
    {
        LanguageService.ChangeLanguage("et");
    }
}
