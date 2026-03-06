using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Mappers.Spotify;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

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