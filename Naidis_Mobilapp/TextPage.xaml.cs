
using Microsoft.Maui.Layouts;

namespace Naidis_Mobilapp;

public partial class TextPage : ContentPage
{
	Label lbl;
	Editor editor;
	HorizontalStackLayout hsl;
    VerticalStackLayout vsl;
    List<string> nupud = new List<string>() { "tagasi", "Avaleht", "Edasi" };
	public TextPage(int i)
	{
		//InitializeComponent();
		lbl = new Label
		{
			Text = "Pealkiri",
			FontSize = 36,
			FontFamily = "BobloxFont",
			TextColor = Colors.Black,
			HorizontalOptions = LayoutOptions.Center,
			FontAttributes = FontAttributes.Bold,
			TextDecorations = TextDecorations.Underline,
		};
		editor = new Editor
		{
			Placeholder = "Sisesta tekst...",
			PlaceholderColor = Colors.Red,
			FontSize = 18,
			FontAttributes = FontAttributes.Italic,
			HorizontalOptions = LayoutOptions.Center,
		};
		editor.TextChanged += (sender, e) =>
		{
			lbl.Text = editor.Text;
		};
		hsl = new HorizontalStackLayout { Spacing = 20, HorizontalOptions = LayoutOptions.Center };
		for (int j = 0; j < nupud.Count; j++)
		{
			Button nupp = new Button
			{
				Text = nupud[j],
				FontSize = 18,
				FontFamily = "BobloxFont",
				BackgroundColor = Colors.LightGray,
				CornerRadius = 10,
				HeightRequest = 40,
				ZIndex = j
			};
			hsl.Add(nupp);
			nupp.Clicked += Liikumine;
		};

		vsl = new VerticalStackLayout
		{
			Padding = 20,
			Spacing = 15,
			Children = { lbl, editor, hsl},
			HorizontalOptions = LayoutOptions.Center
		};
	}

    private void Liikumine(object? sender, EventArgs e)
    {
		Button nupp = sender as Button;
		if (nupp.ZIndex == 0)
		{
			Navigation.PopAsync();
		}
		else if (nupp.ZIndex == 1)
		{
			Navigation.PopToRootAsync();
		}
		else if (nupp.ZIndex == 2)
		{
			Navigation.PushAsync(new FigurePage());
		}
    }
}