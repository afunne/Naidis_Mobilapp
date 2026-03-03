using System.Threading.Tasks;

namespace Naidis_Mobilapp;

public partial class StartPage : ContentPage
{
	VerticalStackLayout vst;
	ScrollView sv;
	public List<ContentPage> Lehed = new List<ContentPage>() { new TextPage(), new FigurePage(), new Timer_Page(), new StepperSliderPage(), new DateTimePage(), new ValgusfoorPage(), new LumememmPage() };
	public List<string> LeheNimed = new List<string>() { "Tekst", "Kujud", "Taimer", "RGB Liugurid", "Kuupäev ja Aeg", "Valgusfoor", "Lumememm" };
	public StartPage()
	{
		Title = "Avaleht";
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