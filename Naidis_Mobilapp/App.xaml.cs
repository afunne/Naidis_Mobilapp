using Microsoft.Extensions.DependencyInjection;

using Naidis_Mobilapp.Services;

namespace Naidis_Mobilapp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            LanguageService.ApplySavedLanguage();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var menuPage = new StartPage();
            var navPage = new NavigationPage(menuPage)
            {
                BarBackgroundColor = Colors.Blue,
                BarTextColor = Colors.WhiteSmoke
            };
            return new Window(navPage);
        }


    }
}
