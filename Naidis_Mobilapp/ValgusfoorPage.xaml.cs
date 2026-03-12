namespace Naidis_Mobilapp;

public partial class ValgusfoorPage : ContentPage
{
    // Olekud
    bool foorSees    = false;
    bool ooRezsiim   = false;
    bool autoRezsiim = false;
    CancellationTokenSource? autoCts;

    // Värvid
    readonly Color hallOn  = Color.FromRgb(180, 180, 180);
    readonly Color hallOff = Color.FromRgb(50,  50,  50);

    Color punaneVarv
    {
        get
        {
            if (ooRezsiim) return Color.FromRgb(160, 0, 0);
            else return Color.FromRgb(255, 50, 50);
        }
    }
    Color kollaneVarv
    {
        get
        {
            if (ooRezsiim) return Color.FromRgb(160, 130, 0);
            else return Color.FromRgb(255, 230, 0);
        }
    }
    Color rohelineVarv
    {
        get
        {
            if (ooRezsiim) return Color.FromRgb(0, 120, 0);
            else return Color.FromRgb(50, 230, 50);
        }
    }
    Color hallVarv
    {
        get
        {
            if (ooRezsiim) return hallOff;
            else return hallOn;
        }
    }
    Color taustVarv
    {
        get
        {
            if (ooRezsiim) return Color.FromRgb(10, 10, 10);
            else return Color.FromRgb(60, 60, 60);
        }
    }
    Color taustaVarv
    {
        get
        {
            if (ooRezsiim) return Colors.Black;
            else return Color.FromRgb(220, 220, 220);
        }
    }

    public ValgusfoorPage()
    {
        InitializeComponent();

        // Tap gesture'id (ainult töötavad kui foor sees)
        LisaTap(punane,   lblPunane,   Colors.Red,       "Seisa! 🛑");
        LisaTap(kollane,  lblKollane,  Colors.Yellow,    "Valmista! 🟡");
        LisaTap(roheline, lblRoheline, Colors.LimeGreen, "Sõida! 🟢");

        VärskendaHallid();
    }

    void LisaTap(Microsoft.Maui.Controls.Shapes.Ellipse ellips, Label lbl, Color varv, string tekst)
    {
        var tap = new TapGestureRecognizer();
        tap.Tapped += async (s, e) =>
        {
            if (!foorSees) { statusLabel.Text = "Lülita esmalt foor sisse"; return; }
            await Task.WhenAll(ellips.ScaleToAsync(1.2, 120), ellips.FadeToAsync(0.5, 120));
            await Task.WhenAll(ellips.ScaleToAsync(1.0, 120), ellips.FadeToAsync(1.0, 120));
            statusLabel.Text = tekst;
        };
        ellips.GestureRecognizers.Add(tap);
        lbl.GestureRecognizers.Add(tap);
    }

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
        vsl.BackgroundColor   = taustaVarv;
        BackgroundColor       = taustaVarv;
        statusLabel.TextColor = ooRezsiim ? Colors.White : Colors.Black;
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

        var tsüklid = new (Microsoft.Maui.Controls.Shapes.Ellipse tuliEl, Color varv, string tekst)[]
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
                punane.Fill   = new SolidColorBrush(hallVarv);
                kollane.Fill  = new SolidColorBrush(hallVarv);
                roheline.Fill = new SolidColorBrush(hallVarv);

                var (tuliEl, varv, tekst) = tsüklid[i % 3];
                tuliEl.Fill      = new SolidColorBrush(varv);
                statusLabel.Text = tekst;

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

    private async void OnTagasiClicked(object? sender, EventArgs e)
        => await Navigation.PopAsync();
}
