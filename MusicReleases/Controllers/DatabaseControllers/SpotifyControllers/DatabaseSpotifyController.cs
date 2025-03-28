using JakubKastner.MusicReleases.Base;
using Tavenem.Blazor.IndexedDB;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers.SpotifyControllers;

public class DatabaseSpotifyController(IndexedDbService dbService) : IDatabaseSpotifyController
{
	private readonly IndexedDbService _dbService = dbService;

	public async Task DeleteAll()
	{
		await _dbService.DeleteDatabaseAsync(SpotifyReleasesDb.Name);
	}

	public IndexedDb GetDb()
	{
		var indexedDb = new IndexedDb(SpotifyReleasesDb.Name, _dbService, SpotifyReleasesDb.GetAllTables(), SpotifyReleasesDb.Version);
		return indexedDb;
	}

	public IndexedDbStore GetTable(Enums.DbStorageTablesSpotify tableName, IndexedDb? db = null)
	{
		db ??= GetDb();
		var table = db[tableName.ToString()];

		if (table is null)
		{
			throw new NullReferenceException(nameof(table));
		}

		return table;
	}
}
