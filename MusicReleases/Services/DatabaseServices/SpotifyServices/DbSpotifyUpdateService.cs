using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyUpdateService(IDbSpotifyService dbService) : IDbSpotifyUpdateService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<DateTime> Get(string userId, LoadingType dbType)
	{
		var key = GetKey(userId, dbType);

		var db = await _dbService.GetDb();
		var meta = await db.Update.Get(key);

		var lastUpdate = meta?.LastUpdate ?? DateTime.MinValue;

		return lastUpdate;
	}

	public async Task SetLastArtistSync(string userId, LoadingType dbType)
	{
		var key = GetKey(userId, dbType);

		var db = await _dbService.GetDb();
		await db.Update.Put(new SpotifyUserUpdateEntity(key, DateTime.Now));
	}

	private static string GetKey(string userId, LoadingType dbType)
	{
		return $"{dbType}_{userId}";
	}
}