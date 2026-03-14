using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public class DbSpotifyUserService(IDbSpotifyService dbService) : IDbSpotifyUserService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<SpotifyUser?> Get(string userId)
	{
		var db = await _dbService.GetDb();

		var userDb = await db.User.Get(userId);
		if (userDb is null)
		{
			return null;
		}

		var user = userDb.ToModel();
		return user;
	}

	public async Task Save(SpotifyUser user)
	{
		var db = await _dbService.GetDb();
		var userDb = user.ToEntity();

		await db.User.PutSafe(userDb);
	}

	public async Task Delete(string userId)
	{
		var db = await _dbService.GetDb();

		await db.User.Delete(userId);
		await DeleteAllUserDatabases(userId);
	}

	private async Task DeleteAllUserDatabases(string userId)
	{
		// TODO delete other dbs

	}
}
