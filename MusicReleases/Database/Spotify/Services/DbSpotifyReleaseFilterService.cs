using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;
using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public class DbSpotifyReleaseFilterService(IDbSpotifyService dbService) : IDbSpotifyReleaseFilterService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<SpotifyReleaseFilter?> Get(string userId)
	{
		var db = await _dbService.GetDb();

		var filterDb = await db.UserFilterRelease.Get(userId);
		var filter = filterDb?.ToModel();

		return filter;
	}

	public async Task Save(SpotifyReleaseFilter filter, string userId)
	{
		var db = await _dbService.GetDb();
		var filterDb = filter.ToEntity(userId);

		await db.UserFilterRelease.PutSafe(filterDb);
	}

	public async Task Delete(string userId)
	{
		var db = await _dbService.GetDb();

		await db.UserFilterRelease.Delete(userId);
	}
}