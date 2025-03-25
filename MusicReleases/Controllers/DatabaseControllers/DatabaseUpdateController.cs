using JakubKastner.MusicReleases.Entities.Api.Spotify.User;
using Tavenem.Blazor.IndexedDB;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public class DatabaseUpdateController(IDatabaseController dbController) : IDatabaseUpdateController
{
	private readonly IDatabaseController _dbController = dbController;

	public async Task<SpotifyLastUpdateEntity> GetOrCreate(IndexedDb db, string userId)
	{
		var userUpdate = await Get(db, userId);

		// record existed
		if (userUpdate is not null)
		{
			return userUpdate;
		}

		// 0 records - add new
		userUpdate = new()
		{
			UserId = userId,
		};

		Console.WriteLine("create update");
		var table = _dbController.GetTable(db, DbStorageTablesSpotify.SpotifyUpdate);
		await table.StoreAsync(userUpdate);

		return userUpdate;
	}

	public async Task<SpotifyLastUpdateEntity?> Get(IndexedDb db, string userId)
	{
		Console.WriteLine(value: "get update");
		var table = _dbController.GetTable(db, DbStorageTablesSpotify.SpotifyUpdate);

		var userUpdatesDb = table.GetAllAsync<SpotifyLastUpdateEntity>();
		await foreach (var userUpdateDb in userUpdatesDb)
		{
			if (userUpdateDb.UserId == userId)
			{
				return userUpdateDb;
			}
		}

		/*var userUpdate = await table.Query<SpotifyLastUpdateEntity>().FirstOrDefaultAsync(x => x.UserId == userId);
		return userUpdate;*/
		return null;
	}

	public async Task Delete(string userId)
	{
		var db = _dbController.GetDb();

		var userUpdateDb = Get(db, userId);

		if (userUpdateDb is null)
		{
			return;
		}
		var table = _dbController.GetTable(db, DbStorageTablesSpotify.SpotifyUpdate);
		await table.ClearAsync();
	}
}
