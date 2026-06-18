using Naidis_Mobilapp.Models;
using SQLite;

namespace Naidis_Mobilapp.Services;

public class CityDatabaseService
{
    readonly SQLiteAsyncConnection database;

    public CityDatabaseService()
    {
        string databasePath = Path.Combine(FileSystem.AppDataDirectory, "cityexplorer.db3");
        database = new SQLiteAsyncConnection(databasePath);
        database.CreateTableAsync<CityPlace>().Wait();
    }

    public Task<List<CityPlace>> GetFavoritesAsync()
    {
        return database.Table<CityPlace>().OrderBy(place => place.NameKey).ToListAsync();
    }

    public Task<int> SaveFavoriteAsync(CityPlace place)
    {
        return database.InsertOrReplaceAsync(place);
    }

    public Task<int> DeleteFavoriteAsync(CityPlace place)
    {
        return database.DeleteAsync(place);
    }

    public async Task<bool> IsFavoriteAsync(string id)
    {
        CityPlace? place = await database.Table<CityPlace>().Where(item => item.Id == id).FirstOrDefaultAsync();
        return place != null;
    }
}
