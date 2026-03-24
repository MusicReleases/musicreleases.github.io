using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Spotify.User;
using JakubKastner.SpotifyApi.User;
using System.Linq.Expressions;

namespace JakubKastner.MusicReleases.Spotify.Settings.User;

internal class SpotifyUserSettingsDbService(IDbSpotifyService dbService, ISpotifyUserClient userClient) : SpotifyUserScopedEntityService<SpotifySettings, SpotifyUserSettingsEntity>(userClient), ISpotifyUserSettingsDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override Expression<Func<SpotifyUserSettingsEntity, string>> UserIdExpression => x => x.UserId;

	protected override string GetEntityUserId(SpotifyUserSettingsEntity entity) => entity.UserId;

	protected override SpotifyUserSettingsEntity ToEntity(SpotifySettings model, string userId) => model.ToEntity(userId);

	protected override SpotifySettings ToModel(SpotifyUserSettingsEntity entity) => entity.ToModel();

	protected override async Task<Table<SpotifyUserSettingsEntity, string>> GetTable()
	{
		var db = await _dbService.GetDb();

		return db.UserSettings;
	}
}