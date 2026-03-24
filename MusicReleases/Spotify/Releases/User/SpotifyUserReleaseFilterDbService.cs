using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.SpotifyApi.User;
using System.Linq.Expressions;

namespace JakubKastner.MusicReleases.Spotify.Releases.User;

internal sealed class SpotifyUserReleaseFilterDbService(IDbSpotifyService dbService, ISpotifyUserClient spotifyUserClient) : SpotifyUserScopedEntityService<SpotifyReleaseFilter, SpotifyUserFilterReleaseEntity>(spotifyUserClient), ISpotifyUserReleaseFilterDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override Expression<Func<SpotifyUserFilterReleaseEntity, string>> UserIdExpression => x => x.UserId;

	protected override string GetEntityUserId(SpotifyUserFilterReleaseEntity entity) => entity.UserId;

	protected override SpotifyUserFilterReleaseEntity ToEntity(SpotifyReleaseFilter model, string userId) => model.ToEntity(userId);

	protected override SpotifyReleaseFilter ToModel(SpotifyUserFilterReleaseEntity entity) => entity.ToModel();

	protected override async Task<Table<SpotifyUserFilterReleaseEntity, string>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.UserFilterRelease;
	}
}