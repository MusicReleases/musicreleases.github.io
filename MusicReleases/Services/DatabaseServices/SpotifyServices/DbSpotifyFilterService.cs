using JakubKastner.MusicReleases.Entities.Api.Spotify.User;
using JakubKastner.MusicReleases.Objects;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyFilterService(IDbSpotifyService dbService) : IDbSpotifyFilterService
{
	private readonly IndexedDbStore _dbTable = dbService.GetTable(DbStorageTablesSpotify.SpotifyFilter);

	public async Task<SpotifyFilter?> Get(string userId)
	{
		var filterDb = await GetDb(userId);
		if (filterDb is null)
		{
			return null;
		}

		var advancedFilter = new SpotifyFilterAdvanced(filterDb);
		var filter = new SpotifyFilter(filterDb, advancedFilter);
		return filter;
	}

	private async Task<SpotifyFilterEntity?> GetDb(string userId)
	{
		Console.WriteLine(value: "get filter");
		var filterDb = await _dbTable.GetItemAsync<SpotifyFilterEntity>(userId);
		return filterDb;
	}

	public async Task Delete(string userId)
	{
		Console.WriteLine("delete filter");
		await _dbTable.RemoveItemAsync(userId);
	}

	public async Task Save(SpotifyFilter filter, string userId)
	{
		Console.WriteLine("save filter");
		var filterDb = new SpotifyFilterEntity(filter, userId);
		await SaveDb(filterDb);
	}

	private async Task SaveDb(SpotifyFilterEntity filterDb)
	{
		Console.WriteLine("save filter");
		await _dbTable.StoreItemAsync(filterDb);
	}
}
