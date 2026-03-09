using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;
using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public class DbSpotifyUpdateService(IDbSpotifyService dbService) : IDbSpotifyUpdateService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<DateTime> Get(string userId, SpotifyDbUpdateType updateType)
	{
		var db = await _dbService.GetDb();
		var key = SpotifyUserUpdateEntity.MakeKey(userId, updateType);

		var meta = await db.UserUpdate.Get(key);

		var lastUpdate = meta?.LastUpdate ?? DateTime.MinValue;
		return lastUpdate;
	}

	public async Task Save(string userId, SpotifyDbUpdateType dbType)
	{
		var db = await _dbService.GetDb();
		var entity = userId.ToSpotifyUpdateEntity(dbType);
		await db.UserUpdate.PutSafe(entity);
	}
}