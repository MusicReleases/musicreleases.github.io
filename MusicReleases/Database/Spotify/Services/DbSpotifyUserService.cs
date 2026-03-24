using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.BaseServices;
using JakubKastner.MusicReleases.Database.Spotify.Links;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Database.Spotify.Services;

internal class DbSpotifyUserService(IDbSpotifyService dbService, IEnumerable<ISpotifyUserLinkEntityService> spotifyUserLinkEntityService, IEnumerable<ISpotifyUserScopedEntityService> spotifyUserScopedEntityService) : IDbSpotifyUserService
{
	private readonly IDbSpotifyService _dbService = dbService;

	private readonly IEnumerable<ISpotifyUserLinkEntityService> _spotifyUserLinkEntityService = spotifyUserLinkEntityService;
	private readonly IEnumerable<ISpotifyUserScopedEntityService> _spotifyUserScopedEntityService = spotifyUserScopedEntityService;

	public async Task<SpotifyUser?> Get(string userId, DateTime lastUpdate)
	{
		var db = await _dbService.GetDb();

		var userDb = await db.User.Get(userId);
		if (userDb is null)
		{
			return null;
		}

		var user = userDb.ToModel(lastUpdate);
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
		await Task.WhenAll(_spotifyUserLinkEntityService.Select(s => s.DeleteAllForUser(userId)));
		await Task.WhenAll(_spotifyUserScopedEntityService.Select(s => s.DeleteByUserId(userId, default)));
	}
}
