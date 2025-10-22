using JakubKastner.MusicReleases.Entities.Api.Spotify.User;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyUpdateService(IDbSpotifyService dbService) : IDbSpotifyUpdateService
{
	private readonly IndexedDbStore _dbTable = dbService.GetTable(DbStorageTablesSpotify.SpotifyUpdate);

	public async Task<SpotifyLastUpdateEntity> GetOrCreate(string userId)
	{
		var userUpdateDb = await Get(userId);

		// record existed
		if (userUpdateDb is not null)
		{
			return userUpdateDb;
		}

		// 0 records - add new
		userUpdateDb = new(userId);

		await Update(userUpdateDb);

		return userUpdateDb;
	}

	public async Task<SpotifyLastUpdateEntity?> Get(string userId)
	{
		Console.WriteLine("db: get update - start");
		var userUpdateDb = await _dbTable.GetItemAsync<SpotifyLastUpdateEntity>(userId);
		Console.WriteLine("db: get update - end");
		return userUpdateDb;
	}

	public async Task Delete(string userId)
	{
		Console.WriteLine("db: delete update - start");
		await _dbTable.RemoveItemAsync(userId);
		Console.WriteLine("db: delete update - end");
	}

	public async Task Update(SpotifyLastUpdateEntity lastUpdateDb)
	{
		Console.WriteLine("db: save update - start");
		await _dbTable.StoreItemAsync(lastUpdateDb);
		Console.WriteLine("db: save update - end");
	}
}
