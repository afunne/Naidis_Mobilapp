using Microsoft.Maui.Controls.Shapes;

namespace Naidis_Mobilapp;

public partial class ValgusfoorPage : ContentPage
{
    // Olekud
    bool foorSees       = false;
    bool ooRezsiim      = false;
    bool autoRezsiim    = false;
    CancellationTokenSource? autoCts;

    // Värvid
    readonly Color hallOn  = Color.FromRgb(180, 180, 180);
    readonly Color hallOff = Color.FromRgb(50,  50,  50);

    Color punaneVarv   => ooRezsiim ? Color.FromRgb(160, 0,   0)   : Color.FromRgb(255, 50,  50);
    Color kollaneVarv  => ooRezsiim ? Color.FromRgb(160, 130, 0)   : Color.FromRgb(255, 230, 0);
    Color rohelineVarv => ooRezsiim ? Color.FromRgb(0,   120, 0)   : Color.FromRgb(50,  230, 50);
    Color hallVarv     => ooRezsiim ? hallOff : hallOn;
    Color taustVarv    => ooRezsiim ? Color.FromRgb(10, 10, 10)  : Color.FromRgb(60, 60, 60);
    Color taustaVarv   => ooRezsiim ? Colors.Black               : Color.FromRgb(220, 220, 220);

    // UI elemendid
    Ellipse  punane, kollane, roheline;
    Label    lblPunane, lblKollane, lblRoheline;
    Label    statusLabel = null!;
    Button   btnSisse = null!, btnValja = null!, btnAuto = null!, btnOo = null!, btnTagasi = null!;
    VerticalStackLayout vsl = null!;

    public ValgusfoorPage()
    {
        InitializeComponent();
        Title = "Valgusfoor";
        BuildUI();
    }

    void BuildUI()
    {
        // Pealkiri
        statusLabel = new Label
        {
            Text              = "Vali valgus",
            FontSize          = 24,
            FontAttributes    = FontAttributes.Bold,
            TextColor         = Colors.White,
            HorizontalOptions = LayoutOptions.Center,
            Margin            = new Thickness(0, 10, 0, 10)
        };

        // Foorikast
        var foorikast = new Border
        {
            BackgroundColor   = taustVarv,
            Padding           = new Thickness(30, 20),
            HorizontalOptions = LayoutOptions.Center,
            WidthRequest      = 160,
            StrokeThickness   = 3,
            Stroke            = Colors.DarkGray
        };
        foorikast.StrokeShape = new RoundRectangle { CornerRadius = 20 };

        var foorVsl = new VerticalStackLayout { Spacing = 15 };

        // Kolm tuld
        (punane,  lblPunane)  = LooTuli("punane");
        (kollane, lblKollane) = LooTuli("kollane");
        (roheline,lblRoheline)= LooTuli("roheline");

        // Tap gesture'id (ainult töötavad kui foor sees)
        LisaTap(punane,   lblPunane,   Colors.Red,       "Seisa! 🛑");
        LisaTap(kollane,  lblKollane,  Colors.Yellow,    "Valmista! 🟡");
        LisaTap(roheline, lblRoheline, Colors.LimeGreen, "Sõida! 🟢");

        foorVsl.Add(TuliKonteiner(punane,   lblPunane));
        foorVsl.Add(TuliKonteiner(kollane,  lblKollane));
        foorVsl.Add(TuliKonteiner(roheline, lblRoheline));
        foorikast.Content = foorVsl;

        // Nupud
        btnSisse  = LooNupp("SISSE",        Colors.DarkGreen,  Colors.White);
        btnValja  = LooNupp("VÄLJA",        Colors.DarkRed,    Colors.White);
        btnAuto   = LooNupp("⏱ Automaat",  Colors.DarkOrange, Colors.White);
        btnOo     = LooNupp("🌙 Öörežiim", Colors.DarkSlateBlue, Colors.White);
        btnTagasi = LooNupp("← Tagasi",    Colors.Gray,       Colors.White);

        btnSisse.Clicked  += OnSisseClicked;
        btnValja.Clicked  += OnValjaClicked;
        btnAuto.Clicked   += OnAutoClicked;
        btnOo.Clicked     += OnOoClicked;
        btnTagasi.Clicked += async (s, e) => await Navigation.PopAsync();

        var nupuRida1 = new HorizontalStackLayout
        {
            Spacing           = 12,
            HorizontalOptions = LayoutOptions.Center,
            Children          = { btnSisse, btnValja }
        };
        var nupuRida2 = new HorizontalStackLayout
        {
            Spacing           = 12,
            HorizontalOptions = LayoutOptions.Center,
            Children          = { btnAuto, btnOo, btnTagasi }
        };

        // Pealeht
        vsl = new VerticalStackLayout
        {
            Spacing           = 15,
            Padding           = new Thickness(20),
            BackgroundColor   = Color.FromRgb(220, 220, 220),
            Children          = { statusLabel, foorikast, nupuRida1, nupuRida2 }
        };

        Content = new ScrollView { Content = vsl };
        VärskendaHallid();
    }

    // Abimeetodid UI loomiseks

    (Ellipse ellips, Label lbl) LooTuli(string tekst)
    {
        var e = new Ellipse
        {
            WidthRequest  = 90,
            HeightRequest = 90,
            Fill          = new SolidColorBrush(hallVarv),
            Stroke        = new SolidColorBrush(Colors.DarkGray),
            StrokeThickness = 3
        };
        var l = new Label
        {
            Text              = tekst,
            TextColor         = Colors.White,
            FontAttributes    = FontAttributes.Bold,
            FontSize          = 14,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions   = LayoutOptions.Center
        };
        return (e, l);
    }

    Grid TuliKonteiner(Ellipse ellips, Label lbl)
    {
        var g = new Grid
        {
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions   = LayoutOptions.Center
        };
        g.Children.Add(ellips);
        g.Children.Add(lbl);
        return g;
    }

    void LisaTap(Ellipse ellips, Label lbl, Color varv, string tekst)
    {
        var tap = new TapGestureRecognizer();
        tap.Tapped += async (s, e) =>
        {
            if (!foorSees) { statusLabel.Text = "Lülita esmalt foor sisse"; return; }
            // Animatsioon: suurenda + tuhmista
            await Task.WhenAll(
                ellips.ScaleToAsync(1.2, 120),
                ellips.FadeToAsync(0.5, 120)
            );
            await Task.WhenAll(
                ellips.ScaleToAsync(1.0, 120),
                ellips.FadeToAsync(1.0, 120)
            );
            statusLabel.Text = tekst;
        };
        ellips.GestureRecognizers.Add(tap);
        lbl.GestureRecognizers.Add(tap);
    }

    Button LooNupp(string tekst, Color taust, Color tekstiVarv) => new Button
    {
        Text            = tekst,
        BackgroundColor = taust,
        TextColor       = tekstiVarv,
        FontSize        = 15,
        FontFamily      = "BobloxFont",
        CornerRadius    = 10,
        HeightRequest   = 44,
        Padding         = new Thickness(14, 0)
    };

    // Nupu käsitlejad

    private void OnSisseClicked(object? sender, EventArgs e)
    {
        foorSees = true;
        punane.Fill   = new SolidColorBrush(punaneVarv);
        kollane.Fill  = new SolidColorBrush(kollaneVarv);
        roheline.Fill = new SolidColorBrush(rohelineVarv);
        vsl.BackgroundColor   = taustaVarv;
        BackgroundColor       = taustaVarv;
        statusLabel.TextColor = ooRezsiim ? Colors.White : Colors.Black;
        statusLabel.Text = "Vali valgus";
    }

    private void OnValjaClicked(object? sender, EventArgs e)
    {
        StopAuto();
        foorSees = false;
        VärskendaHallid();
        statusLabel.Text = "Lülita esmalt foor sisse";
    }

    private void VärskendaHallid()
    {
        punane.Fill   = new SolidColorBrush(hallVarv);
        kollane.Fill  = new SolidColorBrush(hallVarv);
        roheline.Fill = new SolidColorBrush(hallVarv);
        vsl.BackgroundColor    = taustaVarv;
        BackgroundColor        = taustaVarv;
        statusLabel.TextColor  = ooRezsiim ? Colors.White : Colors.Black;
    }

    // Automaatrežiim – vahetab tulesid iga 2 sek
    private async void OnAutoClicked(object? sender, EventArgs e)
    {
        if (autoRezsiim)
        {
            StopAuto();
            btnAuto.Text = "⏱ Automaat";
            return;
        }
        if (!foorSees) OnSisseClicked(null, EventArgs.Empty);

        autoRezsiim = true;
        btnAuto.Text = "⏹ Peata";
        autoCts = new CancellationTokenSource();
        var token = autoCts.Token;

        var tsüklid = new (Ellipse tuliEl, Color varv, string tekst)[]
        {
            (punane,   punaneVarv,   "Seisa! 🛑"),
            (kollane,  kollaneVarv,  "Valmista! 🟡"),
            (roheline, rohelineVarv, "Sõida! 🟢")
        };

        try
        {
            int i = 0;
            while (!token.IsCancellationRequested)
            {
                // Kustuta kõik
                punane.Fill   = new SolidColorBrush(hallVarv);
                kollane.Fill  = new SolidColorBrush(hallVarv);
                roheline.Fill = new SolidColorBrush(hallVarv);

                var (tuliEl, varv, tekst) = tsüklid[i % 3];
                tuliEl.Fill      = new SolidColorBrush(varv);
                statusLabel.Text = tekst;

                // Kollase puhul lühema pausiga
                int viide = (i % 3 == 1) ? 1000 : 2000;
                await Task.Delay(viide, token);
                i++;
            }
        }
        catch (TaskCanceledException) { }
    }

    void StopAuto()
    {
        autoRezsiim = false;
        autoCts?.Cancel();
        btnAuto.Text = "⏱ Automaat";
    }

    // Öörežiim
    private void OnOoClicked(object? sender, EventArgs e)
    {
        ooRezsiim = !ooRezsiim;
        btnOo.Text = ooRezsiim ? "☀️ Päevarežiim" : "🌙 Öörežiim";
        if (foorSees)
            OnSisseClicked(null, EventArgs.Empty);
        else
            VärskendaHallid();
    }
}
