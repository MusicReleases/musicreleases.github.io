using JakubKastner.MusicReleases.Base;
using Tavenem.Blazor.IndexedDB;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyService(IndexedDbService indexedDbService) : IDbSpotifyService
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
