using SQLite;

namespace Naidis_Mobilapp.Models;

public class CityPlace
{
    [PrimaryKey]
    public string Id { get; set; } = "";

    public string CategoryKey { get; set; } = "";

    public string CategoryEmoji { get; set; } = "";

    public string NameKey { get; set; } = "";

    public string ShortDescriptionKey { get; set; } = "";

    public string DetailKey { get; set; } = "";

    public string ImageUrl { get; set; } = "";

    [Ignore]
    public string Name => Resources.Localization.AppResources.Get(NameKey);

    [Ignore]
    public string ShortDescription => Resources.Localization.AppResources.Get(ShortDescriptionKey);

    [Ignore]
    public string Detail => Resources.Localization.AppResources.Get(DetailKey);
}
