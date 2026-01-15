using DexieNET;
using JakubKastner.Extensions;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Mappers.Spotify;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyUpdateService(IDbSpotifyService dbService) : IDbSpotifyUpdateService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<DateTime> Get(string userId, SpotifyDbUpdateType dbType)
	{
		var key = userId.ToSpotifyUpdateKey(dbType);

		var db = await _dbService.GetDb();
		var meta = await db.Update.Get(key);

		var lastUpdate = meta?.LastUpdate ?? DateTime.MinValue;
		return lastUpdate;
	}

	public async Task Save(string userId, SpotifyDbUpdateType dbType)
	{
		var db = await _dbService.GetDb();
		await db.Update.PutSafe(userId.ToSpotifyUpdateEntity(dbType));
	}
}