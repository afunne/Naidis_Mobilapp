using Microsoft.Maui.Controls.Shapes;

namespace Naidis_Mobilapp;

public partial class LumememmPage : ContentPage
{
    // Elemendid
    AbsoluteLayout absLayout = null!;
    BoxView ambar = null!;
    BoxView ambarServ = null!;
    Border pea = null!;
    Border keha = null!;
    // Silmad ja nööbid
    Border silmVasakpoolne = null!, silmParempoolne = null!;
    Border nupp1 = null!, nupp2 = null!, nupp3 = null!;
    // Nina ja suu
    Polygon ninaOts = null!;
    // Picker, Label, Slider, Stepper, Button
    Picker tegevusPicker = null!;
    Label  statusLabel   = null!;
    Slider opacitySlider = null!;
    Stepper kiirusStepper = null!;
    Label  kiirusLabel   = null!;
    Label  opacityLabel  = null!;
    // Öörežiim
    bool ooRezsiim = false;
    ScrollView scrollView = null!;
    Image bgImage = null!;

    // Kõik lumememme osad ühes listis (lihtsam läbipaistvust muuta)
    List<View> memmeOsad = null!;

    readonly List<string> tegevused = new()
    {
        "Peida lumememm",
        "Näita lumememm",
        "Muuda värvi",
        "Sulata",
        "Tantsi",
        "TextToSpeech"
    };

    public LumememmPage()
    {
        InitializeComponent();
        BuildUI();
    }

    void BuildUI()
    {
        // AbsoluteLayout lumememme jaoks
        absLayout = new AbsoluteLayout
        {
            HeightRequest     = 380,
            HorizontalOptions = LayoutOptions.Center,
            WidthRequest      = 260
        };

        // Ämber (must silinder-kübara stiilis)
        // Kübara keha (kitsam, pikk, ümar ülaots)
        ambar = new BoxView
        {
            Color         = Colors.Black,
            CornerRadius  = new CornerRadius(5, 5, 0, 0),
            HeightRequest = 50,
            WidthRequest  = 72
        };
        AbsoluteLayout.SetLayoutBounds(ambar, new Rect(0.5, 0.0, 72, 50));
        AbsoluteLayout.SetLayoutFlags(ambar,
            Microsoft.Maui.Layouts.AbsoluteLayoutFlags.XProportional |
            Microsoft.Maui.Layouts.AbsoluteLayoutFlags.YProportional);

        // Kübara serv (lai, lame — istub kübara ALLäärde / pea peal)
        ambarServ = new BoxView
        {
            Color         = Colors.Black,
            CornerRadius  = new CornerRadius(4),
            HeightRequest = 10,
            WidthRequest  = 100
        };
        AbsoluteLayout.SetLayoutBounds(ambarServ, new Rect(0.5, 0.135, 100, 10));
        AbsoluteLayout.SetLayoutFlags(ambarServ,
            Microsoft.Maui.Layouts.AbsoluteLayoutFlags.XProportional |
            Microsoft.Maui.Layouts.AbsoluteLayoutFlags.YProportional);

        // Pea (väiksem ring)
        pea = new Border
        {
            BackgroundColor = Colors.White,
            HeightRequest   = 110,
            WidthRequest    = 110,
            Padding         = 0,
            StrokeThickness = 1,
            Stroke          = Colors.LightGray
        };
        pea.StrokeShape = new Ellipse { WidthRequest = 110, HeightRequest = 110 };
        AbsoluteLayout.SetLayoutBounds(pea, new Rect(0.5, 0.15, 110, 110));
        AbsoluteLayout.SetLayoutFlags(pea,
            Microsoft.Maui.Layouts.AbsoluteLayoutFlags.XProportional |
            Microsoft.Maui.Layouts.AbsoluteLayoutFlags.YProportional);

        // Silmad
        silmVasakpoolne = LooViksRing(Colors.Black, 14);
        AbsoluteLayout.SetLayoutBounds(silmVasakpoolne, new Rect(0.35, 0.23, 14, 14));
        AbsoluteLayout.SetLayoutFlags(silmVasakpoolne,
            Microsoft.Maui.Layouts.AbsoluteLayoutFlags.XProportional |
            Microsoft.Maui.Layouts.AbsoluteLayoutFlags.YProportional);

        silmParempoolne = LooViksRing(Colors.Black, 14);
        AbsoluteLayout.SetLayoutBounds(silmParempoolne, new Rect(0.65, 0.23, 14, 14));
        AbsoluteLayout.SetLayoutFlags(silmParempoolne,
            Microsoft.Maui.Layouts.AbsoluteLayoutFlags.XProportional |
            Microsoft.Maui.Layouts.AbsoluteLayoutFlags.YProportional);

        // Nina (porgand — ainult Polygon kolmnurk, pikem)
        ninaOts = new Polygon
        {
            Fill            = new SolidColorBrush(Colors.OrangeRed),
            StrokeThickness = 0,
            Points          = new PointCollection { new Point(0,0), new Point(0,12), new Point(38,6) },
            WidthRequest    = 38,
            HeightRequest   = 12
        };
        AbsoluteLayout.SetLayoutBounds(ninaOts, new Rect(0.57, 0.295, 38, 12));
        AbsoluteLayout.SetLayoutFlags(ninaOts,
            Microsoft.Maui.Layouts.AbsoluteLayoutFlags.XProportional |
            Microsoft.Maui.Layouts.AbsoluteLayoutFlags.YProportional);

        // Keha (suurem ring)
        keha = new Border
        {
            BackgroundColor = Colors.White,
            HeightRequest   = 160,
            WidthRequest    = 160,
            Padding         = 0,
            StrokeThickness = 1,
            Stroke          = Colors.LightGray
        };
        keha.StrokeShape = new Ellipse { WidthRequest = 160, HeightRequest = 160 };
        AbsoluteLayout.SetLayoutBounds(keha, new Rect(0.5, 0.56, 160, 160));
        AbsoluteLayout.SetLayoutFlags(keha,
            Microsoft.Maui.Layouts.AbsoluteLayoutFlags.XProportional |
            Microsoft.Maui.Layouts.AbsoluteLayoutFlags.YProportional);

        // Nööbid kehale
        nupp1 = LooViksRing(Colors.Black, 16);
        nupp2 = LooViksRing(Colors.Black, 16);
        nupp3 = LooViksRing(Colors.Black, 16);
        AbsoluteLayout.SetLayoutBounds(nupp1, new Rect(0.5, 0.575, 16, 16));
        AbsoluteLayout.SetLayoutBounds(nupp2, new Rect(0.5, 0.635, 16, 16));
        AbsoluteLayout.SetLayoutBounds(nupp3, new Rect(0.5, 0.695, 16, 16));
        foreach (var n in new[] { nupp1, nupp2, nupp3 })
            AbsoluteLayout.SetLayoutFlags(n,
                Microsoft.Maui.Layouts.AbsoluteLayoutFlags.XProportional |
                Microsoft.Maui.Layouts.AbsoluteLayoutFlags.YProportional);

        // Lisa järjekorras (tagumised enne, et peale tulla)
        absLayout.Children.Add(keha);
        absLayout.Children.Add(pea);
        absLayout.Children.Add(ambar);
        absLayout.Children.Add(ambarServ);
        absLayout.Children.Add(silmVasakpoolne);
        absLayout.Children.Add(silmParempoolne);
        absLayout.Children.Add(ninaOts);
        absLayout.Children.Add(nupp1);
        absLayout.Children.Add(nupp2);
        absLayout.Children.Add(nupp3);

        memmeOsad = new List<View>
            { ambar, ambarServ, pea, keha, silmVasakpoolne, silmParempoolne, ninaOts, nupp1, nupp2, nupp3 };

        // Status label
        statusLabel = new Label
        {
            Text              = "Tere, lumememm!",
            FontSize          = 20,
            FontAttributes    = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center,
            TextColor         = Colors.DarkSlateBlue
        };

        // Picker
        tegevusPicker = new Picker
        {
            Title             = "Vali tegevus...",
            HorizontalOptions = LayoutOptions.Fill,
            FontSize          = 16
        };
        foreach (var t in tegevused) tegevusPicker.Items.Add(t);

        var pickerBtn = new Button
        {
            Text            = "▶ Käivita",
            BackgroundColor = Colors.DarkSlateBlue,
            TextColor       = Colors.White,
            CornerRadius    = 10,
            HeightRequest   = 44
        };
        pickerBtn.Clicked += OnTegevusClicked;

        var pickerRida = new Grid
        {
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
                new ColumnDefinition { Width = GridLength.Auto }
            },
            ColumnSpacing = 10
        };
        pickerRida.Add(tegevusPicker, 0, 0);
        pickerRida.Add(pickerBtn,     1, 0);

        // Slider (läbipaistvus)
        opacityLabel = new Label
        {
            Text      = "Läbipaistvus:",
            FontSize  = 14,
            TextColor = Colors.Gray
        };
        opacitySlider = new Slider { Minimum = 0, Maximum = 1, Value = 1 };
        opacitySlider.ValueChanged += (s, e) =>
        {
            foreach (var v in memmeOsad) v.Opacity = e.NewValue;
        };

        // Stepper (kiirus)
        kiirusLabel = new Label
        {
            Text      = "Kiirus: 500 ms",
            FontSize  = 14,
            TextColor = Colors.Gray
        };
        kiirusStepper = new Stepper
        {
            Minimum   = 100,
            Maximum   = 2000,
            Increment = 100,
            Value     = 500
        };
        kiirusStepper.ValueChanged += (s, e) =>
            kiirusLabel.Text = $"Kiirus: {(int)e.NewValue} ms";

        var kiirusRida = new HorizontalStackLayout
        {
            Spacing           = 15,
            HorizontalOptions = LayoutOptions.Center,
            Children          = { kiirusLabel, kiirusStepper }
        };

        // Öörežiim & Tagasi nupud
        var btnOo = new Button
        {
            Text            = "🌙 Öörežiim",
            FontSize        = 15,
            BackgroundColor = Colors.DarkSlateBlue,
            TextColor       = Colors.White,
            CornerRadius    = 10,
            HeightRequest   = 40
        };
        btnOo.Clicked += OnOoClicked;

        var btnTagasi = new Button
        {
            Text            = "← Tagasi",
            FontSize        = 15,
            BackgroundColor = Colors.Gray,
            TextColor       = Colors.White,
            CornerRadius    = 10,
            HeightRequest   = 40
        };
        btnTagasi.Clicked += async (s, e) => await Navigation.PopAsync();

        var alumineRida = new HorizontalStackLayout
        {
            Spacing           = 12,
            HorizontalOptions = LayoutOptions.Center,
            Children          = { btnOo, btnTagasi }
        };

        // Lumememm raam koos talvetausta pildiga
        // Grid kihib taustapildi ja lumememme üksteise peale
        var lumememGrid = new Grid
        {
            HorizontalOptions = LayoutOptions.Center,
            WidthRequest      = 280,
            HeightRequest     = 400
        };

        bgImage = new Image
        {
            Source  = "winterbg.png",
            Aspect  = Aspect.AspectFill,
            HorizontalOptions = LayoutOptions.Fill,
            VerticalOptions   = LayoutOptions.Fill
        };

        // Tsentrimi absLayout griidi sees
        absLayout.HorizontalOptions = LayoutOptions.Center;
        absLayout.VerticalOptions   = LayoutOptions.Center;

        lumememGrid.Children.Add(bgImage);
        lumememGrid.Children.Add(absLayout);

        var lumememRaam = new Border
        {
            HorizontalOptions = LayoutOptions.Center,
            StrokeThickness   = 3,
            Stroke            = Colors.SteelBlue,
            Shadow            = new Shadow { Brush = Brush.Black, Offset = new Point(4, 4), Radius = 12, Opacity = 0.4f }
        };
        lumememRaam.StrokeShape = new RoundRectangle { CornerRadius = new CornerRadius(20) };
        lumememRaam.Content     = lumememGrid;

        // Pealeht
        var vsl = new VerticalStackLayout
        {
            Spacing  = 14,
            Padding  = new Thickness(20),
            Children =
            {
                statusLabel,
                lumememRaam,
                pickerRida,
                opacityLabel,
                opacitySlider,
                kiirusRida,
                alumineRida
            }
        };

        scrollView = new ScrollView { Content = vsl };
        Content = scrollView;
    }

    // Abimeetod väikese ringi loomiseks
    Border LooViksRing(Color varv, int suurus)
    {
        var b = new Border
        {
            BackgroundColor = varv,
            HeightRequest   = suurus,
            WidthRequest    = suurus,
            Padding         = 0,
            StrokeThickness = 0
        };
        b.StrokeShape = new Ellipse { WidthRequest = suurus, HeightRequest = suurus };
        return b;
    }

    // Tegevuse käivitamine
    // Abimeetod statusLabel teksti + värvi korraga seadmiseks
    void SetStatus(string tekst)
    {
        statusLabel.Text      = tekst;
        statusLabel.TextColor = ooRezsiim ? Colors.LightCyan : Colors.DarkSlateBlue;
    }

    private async void OnTegevusClicked(object? sender, EventArgs e)
    {
        if (tegevusPicker.SelectedIndex < 0)
        {
            SetStatus("Vali kõigepealt tegevus!");
            return;
        }

        string valitud = tegevusPicker.SelectedItem.ToString()!;
        SetStatus(valitud);
        uint kiirus = (uint)kiirusStepper.Value;

        switch (valitud)
        {
            case "Peida lumememm":
                foreach (var v in memmeOsad) v.IsVisible = false;
                SetStatus("Lumememm on peidetud 👻");
                break;

            case "Näita lumememm":
                foreach (var v in memmeOsad) { v.IsVisible = true; v.Opacity = opacitySlider.Value; }
                SetStatus("Lumememm on nähtav ⛄");
                break;

            case "Muuda värvi":
                bool kinnitus = await DisplayAlertAsync("Muuda värvi",
                    "Kas muuta lumememme värvi?", "Jah", "Ei");
                if (kinnitus)
                {
                    var rnd = new Random();
                    Color uusVarv = Color.FromRgb(rnd.Next(180, 256), rnd.Next(180, 256), rnd.Next(200, 256));
                    pea.BackgroundColor  = uusVarv;
                    keha.BackgroundColor = uusVarv;
                    SetStatus("Värv muudetud! 🎨");
                }
                break;

            case "Sulata":
                SetStatus("Sulab... 💧");
                await Task.WhenAll(
                    absLayout.FadeToAsync(0, kiirus * 3),
                    absLayout.ScaleToAsync(0.3, kiirus * 3)
                );
                SetStatus("Lumememm sulanud 💧");
                absLayout.Opacity = 1;
                absLayout.Scale   = 1;
                foreach (var v in memmeOsad) v.IsVisible = true;
                opacitySlider.Value = 1;
                break;

            case "Tantsi":
                SetStatus("Tantsin! 🕺");
                for (int i = 0; i < 4; i++)
                {
                    await absLayout.TranslateToAsync(-30, 0, kiirus / 2);
                    await absLayout.TranslateToAsync( 30, 0, kiirus / 2);
                }
                await absLayout.TranslateToAsync(0, 0, kiirus / 4);
                SetStatus("Tantsisin ära! 💃");
                break;

            case "TextToSpeech 🎅":
                SetStatus("Ütlen midagi... 🎅");
                await TextToSpeech.SpeakAsync("Jõulud tulevad! Tere lumememm!");
                SetStatus("Jõulud tulevad! 🎄");
                break;
        }
    }

    // Öörežiim
    private void OnOoClicked(object? sender, EventArgs e)
    {
        ooRezsiim = !ooRezsiim;
        if (sender is Button btn)
            btn.Text = ooRezsiim ? "☀️ Päevarežiim" : "🌙 Öörežiim";

        scrollView.BackgroundColor    = ooRezsiim ? Color.FromRgb(15, 15, 30) : Colors.White;
        statusLabel.TextColor         = ooRezsiim ? Colors.LightCyan : Colors.DarkSlateBlue;
        opacityLabel.TextColor        = ooRezsiim ? Colors.White : Colors.Gray;
        kiirusLabel.TextColor         = ooRezsiim ? Colors.White : Colors.Gray;
        tegevusPicker.TextColor       = ooRezsiim ? Colors.White : Colors.Black;
        tegevusPicker.TitleColor      = ooRezsiim ? Colors.LightGray : Colors.Gray;
        tegevusPicker.BackgroundColor = ooRezsiim ? Color.FromRgb(40, 40, 60) : Colors.White;
        pea.BackgroundColor           = ooRezsiim ? Color.FromRgb(200, 220, 255) : Colors.White;
        keha.BackgroundColor          = ooRezsiim ? Color.FromRgb(200, 220, 255) : Colors.White;
        bgImage.Source                = ooRezsiim ? "winterbgnight.jpg" : "winterbg.png";
    }
}
