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

	public async Task Save(string userId, SpotifyDbUpdateType updateType)
	{
		var db = await _dbService.GetDb();
		var entity = userId.ToSpotifyUpdateEntity(updateType);
		await db.UserUpdate.PutSafe(entity);
	}

	public async Task Delete(string userId, SpotifyDbUpdateType updateType)
	{
		var db = await _dbService.GetDb();
		var key = SpotifyUserUpdateEntity.MakeKey(userId, updateType);
		await db.UserUpdate.Delete(key);
	}
}