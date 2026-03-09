using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyUserLinkService(IDbSpotifyService dbService) : IDbSpotifyUserLinkService
{
	private readonly IDbSpotifyService _dbService = dbService;

	private async Task<SpotifyUserLinkEntity?> GetEntity(string userId)
	{
		var db = await _dbService.GetDb();

		var entity = await db.Link.Get(userId);

		return entity;
	}

	private async Task<SpotifyUserLinkEntity> GetOrCreateEntity(string userId)
	{
		var entity = await GetEntity(userId) ?? new(userId, null, null);
		return entity;
	}

	public async Task<string?> GetTasksLink(string userId)
	{
		var entity = await GetEntity(userId);
		return entity?.Tasks;
	}

	public async Task<string?> GetReleasesLink(string userId)
	{
		var entity = await GetEntity(userId);
		return entity?.Releases;
	}

	public async Task SetTasksLink(string userId, string? link)
	{
		var entity = await GetOrCreateEntity(userId) with
		{
			Tasks = link,
		};

		var db = await _dbService.GetDb();
		await db.Link.PutSafe(entity);
	}

	public async Task SetReleasesLink(string userId, string? link)
	{
		var entity = await GetOrCreateEntity(userId) with
		{
			Releases = link,
		};

		var db = await _dbService.GetDb();
		await db.Link.PutSafe(entity);
	}
}
