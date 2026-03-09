using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;
using JakubKastner.MusicReleases.Enums;

namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public class DbSpotifyTaskFilterService(IDbSpotifyService dbService) : IDbSpotifyTaskFilterService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<TaskFilter?> Get(string userId)
	{
		var db = await _dbService.GetDb();

		var filterDb = await db.UserFilterTask.Get(userId);
		var filter = filterDb?.ToModel();

		return filter;
	}

	public async Task Save(TaskFilter filter, string userId)
	{
		var db = await _dbService.GetDb();
		var filterDb = filter.ToEntity(userId);

		await db.UserFilterTask.PutSafe(filterDb);
	}

	public async Task Delete(string userId)
	{
		var db = await _dbService.GetDb();

		await db.UserFilterTask.Delete(userId);
	}
}