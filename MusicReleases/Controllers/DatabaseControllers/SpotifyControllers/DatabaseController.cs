using JakubKastner.MusicReleases.Base;
using Tavenem.Blazor.IndexedDB;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers.SpotifyControllers;

public class DatabaseController(IndexedDbService dbService) : IDatabaseController
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

	public IndexedDbStore GetTable(Enums.DbStorageTablesSpotify tableName)
	{
		var db = GetDb();
		return GetTable(db, tableName);
	}

	public IndexedDbStore GetTable(IndexedDb db, Enums.DbStorageTablesSpotify tableName)
	{
		var table = db[tableName.ToString()];

		if (table is null)
		{
			throw new NullReferenceException(nameof(table));
		}

		return table;
	}
}
