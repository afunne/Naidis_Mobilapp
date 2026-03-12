namespace Naidis_Mobilapp;

public partial class StepperSliderPage : ContentPage
{
    bool _updating = false; // takistab tsüklilist uuendamist

    public StepperSliderPage()
    {
        InitializeComponent();
        UpdateColor();
    }

    // Slider muudab värvi uuendab Entry teksti ka
    private void OnSliderChanged(object sender, ValueChangedEventArgs e)
    {
        if (_updating) return;
        _updating = true;
        entRed.Text   = Convert.ToInt32(sldRed.Value).ToString();
        entGreen.Text = Convert.ToInt32(sldGreen.Value).ToString();
        entBlue.Text  = Convert.ToInt32(sldBlue.Value).ToString();
        _updating = false;
        UpdateColor();
    }

    // Entry tekstimuutus uuendab Sliderit ka
    private void OnEntryChanged(object sender, TextChangedEventArgs e)
    {
        if (_updating) return;
        _updating = true;
        if (int.TryParse(entRed.Text,   out int r)) sldRed.Value   = Math.Clamp(r, 0, 255);
        if (int.TryParse(entGreen.Text, out int g)) sldGreen.Value = Math.Clamp(g, 0, 255);
        if (int.TryParse(entBlue.Text,  out int b)) sldBlue.Value  = Math.Clamp(b, 0, 255);
        _updating = false;
        UpdateColor();
    }

    // HEX koodi rakendamine
    private void OnApplyHexClicked(object sender, EventArgs e)
    {
        if (Color.TryParse(entHex.Text, out Color selectedColor))
        {
            _updating = true;
            int r = (int)(selectedColor.Red * 255);
            int g = (int)(selectedColor.Green * 255);
            int b = (int)(selectedColor.Blue * 255);

            sldRed.Value = r; entRed.Text = r.ToString();
            sldGreen.Value = g; entGreen.Text = g.ToString();
            sldBlue.Value = b; entBlue.Text = b.ToString();

            _updating = false;
            UpdateColor();
            entHex.TextColor = Colors.Black;
        }
        else
        {
            entHex.TextColor = Colors.Red;
        }
    }

    // Nurkade ümarus Stepperiga
    private void OnStepperValueChanged(object sender, ValueChangedEventArgs e)
    {
        lblCornerRadius.Text = e.NewValue.ToString();
        colorBoxShape.CornerRadius = (float)e.NewValue;
    }

    // Teksti suurus (Stepper) ja pöördenurk (Slider)
    private void Stepper_Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        
    }

    private void UpdateColor()
    {
        int r = Convert.ToInt32(sldRed.Value);
        int g = Convert.ToInt32(sldGreen.Value);
        int b = Convert.ToInt32(sldBlue.Value);

        ColorBox.Background  = Color.FromRgb(r, g, b);
        boxRed.Background    = Color.FromRgb(r, 0, 0);
        boxGreen.Background  = Color.FromRgb(0, g, 0);
        boxBlue.Background   = Color.FromRgb(0, 0, b);

        string hex = $"#{r:X2}{g:X2}{b:X2}";
        lblHex.Text      = hex;
        entHex.Text      = hex;
        entHex.TextColor = Colors.Black;

        if (r + g + b > 380)
        {
            lblHex.TextColor = Colors.Black;
        }
        else
        {
            lblHex.TextColor = Colors.White;
        }
    }

    private async void OnRandomColorClicked(object sender, EventArgs e)
    {
        var rnd = new Random();
        // Animatsiooniga liugurid liiguvad järjestikku
        sldRed.Value   = rnd.Next(0, 256);
        await Task.Delay(60);
        sldGreen.Value = rnd.Next(0, 256);
        await Task.Delay(60);
        sldBlue.Value  = rnd.Next(0, 256);
    }

    private async void tagasi_Clicked(object sender, EventArgs e)
        => await Navigation.PopAsync();

    private async void avaleht_Clicked(object sender, EventArgs e)
        => await Navigation.PopToRootAsync();

    private async void edasi_Clicked(object sender, EventArgs e)
        => await Navigation.PushAsync(new TextDemoPage());
}