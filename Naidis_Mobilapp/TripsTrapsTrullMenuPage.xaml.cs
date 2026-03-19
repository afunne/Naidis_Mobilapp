using Microsoft.Maui.Controls;

namespace Naidis_Mobilapp
{
    public partial class TripsTrapsTrullMenuPage : ContentPage
    {
        public TripsTrapsTrullMenuPage()
        {
            InitializeComponent();
        }

        private async void OnStartGameClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TripsTrapsTrullPage());
        }

        private async void OnInfoClicked(object sender, EventArgs e)
        {
            await ShowAlert("Statistika / Reeglid", "Vaata statistikat ja reegleid mängulehel!");
        }

        private async void OnThemeClicked(object sender, EventArgs e)
        {
            var nav = Application.Current.MainPage as NavigationPage;
            if (nav != null && nav.CurrentPage is TripsTrapsTrullPage gamePage)
            {
                gamePage.ToggleTheme();
            }
            else
            {
                await ShowAlert("Teema", "Teema vahetamiseks mine mängulehele!");
            }
        }

        private async Task ShowAlert(string title, string message)
        {
            var popup = new SimpleAlertPopup(title, message);
            await this.ShowPopupAsync(popup);
        }
    }
}
