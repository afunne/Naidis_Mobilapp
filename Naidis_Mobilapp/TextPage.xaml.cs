
using Microsoft.Maui.Layouts;

namespace Naidis_Mobilapp;

public partial class TextPage : ContentPage
{
	Label lbl;
	Editor editor;
	HorizontalStackLayout hsl;
    VerticalStackLayout vsl;
    Stepper stepper;
    Slider slider;
    List<string> nupud = new List<string>() { "tagasi", "Avaleht", "Edasi" };
	public TextPage() // int i
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

		// Stepper - muudab fondi suurust sammhaaval
		stepper = new Stepper
		{
			Minimum = 10,
			Maximum = 72,
			Value = 36,
			Increment = 2,
			HorizontalOptions = LayoutOptions.Center
		};
		stepper.ValueChanged += (sender, e) =>
		{
			lbl.FontSize = stepper.Value;
		};

		// Slider - muudab fondi suurust siledalt
		slider = new Slider
		{
			Minimum = 10,
			Maximum = 72,
			Value = 36,
			HorizontalOptions = LayoutOptions.Fill
		};
		slider.ValueChanged += (sender, e) =>
		{
			lbl.FontSize = slider.Value;
			stepper.Value = slider.Value;
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
		}

		vsl = new VerticalStackLayout
		{
			Padding = 20,
			Spacing = 15,
			Children = { lbl, editor, stepper, slider, hsl },
			HorizontalOptions = LayoutOptions.Center
		};
		Content = vsl;
	}

    private void Liikumine(object? sender, EventArgs e)
    {
		if (sender is not Button nupp) return;
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