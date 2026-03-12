namespace Naidis_Mobilapp;

public partial class LumememmPage : ContentPage
{
    // Öörežiim
    bool ooRezsiim = false;

    // Kõik lumememme osad ühes listis (lihtsam läbipaistvust muuta)
    List<View> memmeOsad = null!;

    public LumememmPage()
    {
        InitializeComponent();
        memmeOsad = new List<View>
            { ambar, ambarServ, pea, keha, silmVasakpoolne, silmParempoolne, ninaOts, nupp1, nupp2, nupp3 };
    }

    // Abimeetod statusLabel teksti + värvi korraga seadmiseks
    void SetStatus(string tekst)
    {
        statusLabel.Text = tekst;
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
        const uint kiirus = 500;

        // Show/hide extra panels based on selection
        colorPickerPanel.IsVisible = valitud == "Muuda värvi";
        ttsEntry.IsVisible         = valitud == "TextToSpeech";
        switch (valitud)
        {
            case "Peida lumememm":
                foreach (var v in memmeOsad) v.IsVisible = false;
                SetStatus("Lumememm on peidetud");
                break;

            case "Näita lumememm":
                foreach (var v in memmeOsad) { v.IsVisible = true; v.Opacity = opacitySlider.Value; }
                SetStatus("Lumememm on nähtav");
                break;

            case "Muuda värvi":
                SetStatus("Vali värv liuguritel!");
                break;

            case "Juhuslik värv":
                var rnd = new Random();
                Color juhuslik = Color.FromRgb(rnd.Next(180, 256), rnd.Next(180, 256), rnd.Next(200, 256));
                pea.BackgroundColor          = juhuslik;
                keha.BackgroundColor         = juhuslik;
                colorPreview.BackgroundColor = juhuslik;
                // sync sliders to the new colour
                sldR.Value = (int)(juhuslik.Red   * 255);
                sldG.Value = (int)(juhuslik.Green * 255);
                sldB.Value = (int)(juhuslik.Blue  * 255);
                SetStatus("Juhuslik värv valitud!");
                break;

            case "Sulata":
                SetStatus("Sulab...");
                await Task.WhenAll(
                    absLayout.FadeToAsync(0, kiirus * 3),
                    absLayout.ScaleToAsync(0.3, kiirus * 3)
                );
                SetStatus("Lumememm sulanud");
                absLayout.Opacity = 1;
                absLayout.Scale   = 1;
                foreach (var v in memmeOsad) v.IsVisible = true;
                opacitySlider.Value = 1;
                break;

            case "Tantsi":
                SetStatus("Tantsin!");
                for (int i = 0; i < 4; i++)
                {
                    await absLayout.TranslateToAsync(-30, 0, kiirus / 2);
                    await absLayout.TranslateToAsync( 30, 0, kiirus / 2);
                }
                await absLayout.TranslateToAsync(0, 0, kiirus / 4);
                SetStatus("Tantsisin ära!");
                break;

            case "TextToSpeech":
                SetStatus("Ütlen midagi...");
                string tekst = string.IsNullOrWhiteSpace(ttsEntry.Text)
                    ? "Jõulud tulevad! Tere lumememm!"
                    : ttsEntry.Text;
                await TextToSpeech.SpeakAsync(tekst);
                SetStatus("Ütlesin ära!");
                break;
        }
    }

    private void OnOpacityChanged(object? sender, ValueChangedEventArgs e)
    {
        foreach (var v in memmeOsad) v.Opacity = e.NewValue;
    }

    private void OnColorSliderChanged(object? sender, ValueChangedEventArgs e)
    {
        int r = (int)sldR.Value;
        int g = (int)sldG.Value;
        int b = (int)sldB.Value;

        lblR.Text = r.ToString();
        lblG.Text = g.ToString();
        lblB.Text = b.ToString();

        Color varv = Color.FromRgb(r, g, b);
        pea.BackgroundColor          = varv;
        keha.BackgroundColor         = varv;
        colorPreview.BackgroundColor = varv;
    }

    private async void OnTagasiClicked(object? sender, EventArgs e)
        => await Navigation.PopAsync();

    // Öörežiim
    private void OnOoClicked(object? sender, EventArgs e)
    {
        ooRezsiim = !ooRezsiim;

        if (ooRezsiim)
        {
            btnOo.Text = "Päevarežiim";
            bgImage.Source = "winterbgnight.jpg";

            scrollView.BackgroundColor = Color.FromRgb(15, 15, 30);
            statusLabel.TextColor = Colors.LightCyan;
            opacityLabel.TextColor = Colors.White;
            tegevusPicker.TextColor = Colors.White;
            tegevusPicker.TitleColor = Colors.LightGray;
            tegevusPicker.BackgroundColor = Color.FromRgb(40, 40, 60);

            Color panelBg = Color.FromRgb(200, 220, 255);
            pea.BackgroundColor  = panelBg;
            keha.BackgroundColor = panelBg;
        }
        else
        {
            btnOo.Text = "Öörežiim";
            bgImage.Source = "winterbg.png";

            scrollView.BackgroundColor = Colors.White;
            statusLabel.TextColor = Colors.DarkSlateBlue;
            opacityLabel.TextColor = Colors.Gray;
            tegevusPicker.TextColor = Colors.Black;
            tegevusPicker.TitleColor = Colors.Gray;
            tegevusPicker.BackgroundColor = Colors.White;

            pea.BackgroundColor  = Colors.White;
            keha.BackgroundColor = Colors.White;
        }
    }
}
