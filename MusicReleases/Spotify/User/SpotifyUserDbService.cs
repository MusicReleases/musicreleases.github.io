using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.BaseServices;
using JakubKastner.MusicReleases.Database.Spotify.Links;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;
using JakubKastner.MusicReleases.Spotify.User.Update;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Spotify.User;

internal class SpotifyUserDbService(IDbSpotifyService dbService, ISpotifyUserUpdateDbService updateDbService, IEnumerable<ISpotifyUserLinkEntityService> linkEntityServices, IEnumerable<ISpotifyUserScopedEntityService> scopedEntityServices) : ISpotifyUserDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	private readonly ISpotifyUserUpdateDbService _updateDbService = updateDbService;
	private readonly IEnumerable<ISpotifyUserLinkEntityService> _linkEntityServices = linkEntityServices;
	private readonly IEnumerable<ISpotifyUserScopedEntityService> _scopedEntityServices = scopedEntityServices;

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
		await _updateDbService.DeleteForUser(userId);

		await Task.WhenAll(_linkEntityServices.Select(s => s.DeleteAllForUser(userId)));
		await Task.WhenAll(_scopedEntityServices.Select(s => s.DeleteByUserId(userId, default)));
	}
}
