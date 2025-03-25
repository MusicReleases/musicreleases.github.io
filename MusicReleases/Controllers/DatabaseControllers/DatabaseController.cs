using JakubKastner.MusicReleases.Base;
using Tavenem.Blazor.IndexedDB;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseController(IndexedDbService indexedDbService) : IDatabaseController
{
	private readonly IndexedDbService _indexedDbService = indexedDbService;

	public async Task DeleteAll()
	{
		await _indexedDbService.DeleteDatabaseAsync(SpotifyReleasesDb.Name);
	}

	public IndexedDb GetDb()
	{
		var indexedDb = new IndexedDb(SpotifyReleasesDb.Name, _indexedDbService, SpotifyReleasesDb.GetAllTables(), SpotifyReleasesDb.Version);
		return indexedDb;
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
