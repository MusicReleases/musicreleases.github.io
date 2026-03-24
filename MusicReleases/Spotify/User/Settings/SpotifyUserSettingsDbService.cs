using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.BaseServices;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;
using JakubKastner.MusicReleases.Objects.User;
using JakubKastner.SpotifyApi.Clients;
using System.Linq.Expressions;

namespace JakubKastner.MusicReleases.Spotify.User.Settings;

internal class SpotifyUserSettingsDbService(IDbSpotifyService dbService, ISpotifyUserClient userClient) : SpotifyUserScopedEntityService<UserSettings, SpotifyUserSettingsEntity>(userClient), ISpotifyUserSettingsDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override Expression<Func<SpotifyUserSettingsEntity, string>> UserIdExpression => x => x.UserId;

	protected override string GetEntityUserId(SpotifyUserSettingsEntity entity) => entity.UserId;

	protected override SpotifyUserSettingsEntity ToEntity(UserSettings model, string userId) => model.ToEntity(userId);

	protected override UserSettings ToModel(SpotifyUserSettingsEntity entity) => entity.ToModel();

	protected override async Task<Table<SpotifyUserSettingsEntity, string>> GetTable()
	{
		var db = await _dbService.GetDb();

		return db.UserSettings;
	}
}