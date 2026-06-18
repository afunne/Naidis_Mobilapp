namespace Naidis_Mobilapp.Models;

public class CityCategory
{
    public string Key { get; set; } = "";

    public string Emoji { get; set; } = "";

    public string TitleKey { get; set; } = "";

    public string Title => Resources.Localization.AppResources.Get(TitleKey);
}
