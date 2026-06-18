using Naidis_Mobilapp.Models;

namespace Naidis_Mobilapp.Services;

public static class CityDataService
{
    public static List<CityCategory> GetCategories()
    {
        return new List<CityCategory>
        {
            new() { Key = "history", Emoji = "\U0001F3F0", TitleKey = "CityCategoryHistory" },
            new() { Key = "parks", Emoji = "\U0001F333", TitleKey = "CityCategoryParks" },
            new() { Key = "food", Emoji = "\U0001F37D", TitleKey = "CityCategoryFood" }
        };
    }

    public static List<CityPlace> GetPlaces()
    {
        return new List<CityPlace>
        {
            new()
            {
                Id = "old-town",
                CategoryKey = "history",
                CategoryEmoji = "\U0001F3F0",
                NameKey = "CityPlaceOldTownName",
                ShortDescriptionKey = "CityPlaceOldTownShort",
                DetailKey = "CityPlaceOldTownDetail",
                ImageUrl = "https://commons.wikimedia.org/wiki/Special:FilePath/Old%20town%20of%20Tallinn%2006-03-2012.jpg"
            },
            new()
            {
                Id = "toompea",
                CategoryKey = "history",
                CategoryEmoji = "\U0001F3F0",
                NameKey = "CityPlaceToompeaName",
                ShortDescriptionKey = "CityPlaceToompeaShort",
                DetailKey = "CityPlaceToompeaDetail",
                ImageUrl = "https://commons.wikimedia.org/wiki/Special:FilePath/Toompea%20castle%2C%20June%202010.jpg"
            },
            new()
            {
                Id = "kadriorg",
                CategoryKey = "parks",
                CategoryEmoji = "\U0001F333",
                NameKey = "CityPlaceKadriorgName",
                ShortDescriptionKey = "CityPlaceKadriorgShort",
                DetailKey = "CityPlaceKadriorgDetail",
                ImageUrl = "https://commons.wikimedia.org/wiki/Special:FilePath/Tallinn%20asv2022-04%20img22%20Kadriorg%20Palace.jpg"
            },
            new()
            {
                Id = "pirita",
                CategoryKey = "parks",
                CategoryEmoji = "\U0001F333",
                NameKey = "CityPlacePiritaName",
                ShortDescriptionKey = "CityPlacePiritaShort",
                DetailKey = "CityPlacePiritaDetail",
                ImageUrl = "https://commons.wikimedia.org/wiki/Special:FilePath/Pirita%20Beach%20Tallinn.jpg"
            },
            new()
            {
                Id = "balti-jaam",
                CategoryKey = "food",
                CategoryEmoji = "\U0001F37D",
                NameKey = "CityPlaceBaltiJaamName",
                ShortDescriptionKey = "CityPlaceBaltiJaamShort",
                DetailKey = "CityPlaceBaltiJaamDetail",
                ImageUrl = "https://commons.wikimedia.org/wiki/Special:FilePath/Balti%20Jaama%20Turg%202021.jpg"
            },
            new()
            {
                Id = "telliskivi",
                CategoryKey = "food",
                CategoryEmoji = "\U0001F37D",
                NameKey = "CityPlaceTelliskiviName",
                ShortDescriptionKey = "CityPlaceTelliskiviShort",
                DetailKey = "CityPlaceTelliskiviDetail",
                ImageUrl = "https://commons.wikimedia.org/wiki/Special:FilePath/Tallinn%20telliskivi%20towards%20fotografiska%20-%202019.jpg"
            }
        };
    }
}
