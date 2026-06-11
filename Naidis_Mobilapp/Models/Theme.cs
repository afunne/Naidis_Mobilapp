namespace Naidis_Mobilapp.Models;

public class Theme
{
    public string Name { get; }
    public Color BackgroundColor { get; }
    public Color SurfaceColor { get; }
    public Color TextColor { get; }
    public Color AccentColor { get; }
    public Color HiddenCellColor { get; }
    public Color OpenCellColor { get; }
    public Color DangerColor { get; }
    public string FontFamily { get; }

    public Theme(
        string name,
        Color backgroundColor,
        Color surfaceColor,
        Color textColor,
        Color accentColor,
        Color hiddenCellColor,
        Color openCellColor,
        Color dangerColor,
        string fontFamily)
    {
        Name = name;
        BackgroundColor = backgroundColor;
        SurfaceColor = surfaceColor;
        TextColor = textColor;
        AccentColor = accentColor;
        HiddenCellColor = hiddenCellColor;
        OpenCellColor = openCellColor;
        DangerColor = dangerColor;
        FontFamily = fontFamily;
    }

    public void Apply(ContentPage page)
    {
        page.Resources["MinesBackgroundColor"] = BackgroundColor;
        page.Resources["MinesSurfaceColor"] = SurfaceColor;
        page.Resources["MinesTextColor"] = TextColor;
        page.Resources["MinesAccentColor"] = AccentColor;
        page.Resources["MinesHiddenCellColor"] = HiddenCellColor;
        page.Resources["MinesOpenCellColor"] = OpenCellColor;
        page.Resources["MinesDangerColor"] = DangerColor;
        page.Resources["MinesFontFamily"] = FontFamily;
        page.BackgroundColor = BackgroundColor;
    }

    public override string ToString()
    {
        return Name;
    }
}
