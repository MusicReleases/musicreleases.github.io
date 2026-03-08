using JakubKastner.MusicReleases.Entities.Api.Spotify.User;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects;
using Tavenem.Blazor.IndexedDB;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyFilterServiceOld(IDbSpotifyServiceOld dbService) : IDbSpotifyFilterServiceOld
{
	private readonly IndexedDbStore _dbTable = dbService.GetTable(DbStorageTablesSpotify.SpotifyFilter);

	public async Task<SpotifyReleaseFilter?> Get(string userId)
	{
		var filterDb = await GetDb(userId);
		if (filterDb is null)
		{
			return null;
		}

		var advancedFilter = new SpotifyFilterAdvanced(filterDb);
		var filter = new SpotifyReleaseFilter(filterDb, advancedFilter);
		return filter;
	}

	private async Task<SpotifyFilterEntityOld?> GetDb(string userId)
	{
		Console.WriteLine("db: get filter - start");
		var filterDb = await _dbTable.GetItemAsync<SpotifyFilterEntityOld>(userId);
		Console.WriteLine("db: get filter - end");
		return filterDb;
	}

	public async Task Delete(string userId)
	{
		Console.WriteLine("db: delete filter - start");
		await _dbTable.RemoveItemAsync(userId);
		Console.WriteLine("db: delete filter - end");
	}

	public async Task Save(SpotifyReleaseFilter filter, string userId)
	{
		var filterDb = new SpotifyFilterEntityOld(filter, userId);
		await SaveDb(filterDb);
	}

	private async Task SaveDb(SpotifyFilterEntityOld filterDb)
	{
		Console.WriteLine("db: save filter");
		await _dbTable.StoreItemAsync(filterDb);
		Console.WriteLine("db: save filter");
	}
}
