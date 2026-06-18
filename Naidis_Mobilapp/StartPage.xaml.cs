using System.Threading.Tasks;

namespace Naidis_Mobilapp;

public partial class StartPage : ContentPage
{
	VerticalStackLayout vst;
	ScrollView sv;
    public List<Page> Lehed = new List<Page>() { new TextPage(), new FigurePage(), new Timer_Page(), new StepperSliderPage(), new TextDemoPage(), new DateTimePage(), new ValgusfoorPage(), new LumememmPage(), new GridDemoPage(), new TripsTrapsTrullPage(), new MinecraftPopupQuizPage(), new SopradeKontaktidPage(), new ListViewTelefonidPage(), new EuroopaRiigidPage(), new ProgrammingLanguagesPage(), new KarussellPage(), new Views.MainPage(), new MinesweeperPage(), new RecipeBookPage(), new Views.CityExplorerPage() };
    public List<string> LeheNimed = new List<string>() { "Tekst", "Kujud", "Taimer", "RGB Liugurid", "Teksti Demo", "Kuupäev ja Aeg", "Valgusfoor", "Lumememm", "Grid Demo", "Trips-Traps-Trull", "Minecraft PopUp Quiz", "Sõprade kontaktandmed", "Telefonide ListView", "Euroopa riigid", "Programmeerimiskeelte portfoolio", "Karussell", "Mitmekeelne rakendus" };
	public StartPage()
	{
		Title = "Avaleht";
        LeheNimed.Add("Miiniväli");
        LeheNimed.Add("Minu digitaalne retseptiraamat");
        LeheNimed.Add("CityExplorer");
		vst = new VerticalStackLayout { Padding=20, Spacing=15};
		for (int i=0; i < Lehed.Count; i++)
        {
            Button nupp = new Button
            {
                Text = LeheNimed[i],
                FontSize = 18,
                FontFamily = "BobloxFont",
                BackgroundColor = Colors.Black,
                CornerRadius = 10,
                HeightRequest = 50,
                ZIndex = i
            };
            vst.Add(nupp);
            nupp.Clicked += (sender, e) =>
            {
                var valik = Lehed[nupp.ZIndex];
                Navigation.PushAsync(valik);
            };
        }
        sv = new ScrollView { Content = vst };
        Content = sv;
    }

    //private static async Task<object> GetNupp_Clicked()
    //{
    //    Button nupp = sender as Button;
    //    await NavigationEventArgs.PushAsync(Lehed[nupp.ZIndex]);
    //}
}
