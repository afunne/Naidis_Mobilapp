using Microsoft.Extensions.DependencyInjection;

namespace Naidis_Mobilapp
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
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
