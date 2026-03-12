namespace Naidis_Mobilapp;

public partial class TextDemoPage : ContentPage
{
    public TextDemoPage()
    {
        InitializeComponent();
    }

    private void OnStepperSliderChanged(object sender, ValueChangedEventArgs e)
    {
        if (lblDemo == null) return;
        lblDemo.FontSize      = stpFontSize.Value;
        lblFontSizeValue.Text = stpFontSize.Value.ToString("0");
        lblDemo.Rotation      = sldRotation.Value;
        lblRotationValue.Text = $"{sldRotation.Value:0}°";
    }

    private async void OnTagasiClicked(object sender, EventArgs e)
        => await Navigation.PopAsync();

    private async void OnAvalehtClicked(object sender, EventArgs e)
        => await Navigation.PopToRootAsync();
}
