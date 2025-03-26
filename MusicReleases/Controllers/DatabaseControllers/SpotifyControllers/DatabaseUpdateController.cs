using JakubKastner.MusicReleases.Entities.Api.Spotify.User;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers.SpotifyControllers;

public class DatabaseUpdateController(IDatabaseController dbController) : IDatabaseUpdateController
{
	private readonly IndexedDbStore _dbTable = dbController.GetTable(DbStorageTablesSpotify.SpotifyUpdate);

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
		Console.WriteLine(value: "get update");
		var userUpdateDb = await _dbTable.GetItemAsync<SpotifyLastUpdateEntity>(userId);
		return userUpdateDb;
	}

	public async Task Delete(string userId)
	{
		Console.WriteLine("delete update");
		await _dbTable.RemoveItemAsync(userId);
	}

	public async Task Update(SpotifyLastUpdateEntity lastUpdateDb)
	{
		Console.WriteLine("save update");
		await _dbTable.StoreItemAsync(lastUpdateDb);
	}
}
