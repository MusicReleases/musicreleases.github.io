using DexieNET;
using JakubKastner.MusicReleases.Mappers.Spotify;
using JakubKastner.MusicReleases.Objects.User;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public class DbSpotifyUserSettingsService(IDbSpotifyService dbService) : IDbSpotifyUserSettingsService
{
	private readonly IDbSpotifyService _dbService = dbService;

	public async Task<UserSettings> Get(string userId)
	{
		var db = await _dbService.GetDb();

		Database.Spotify.Entities.SpotifyUserSettingsEntity? entity = await db.Settings.Get(userId);
		var userSettings = entity is null ? new() : entity.ToModel();

		return userSettings;
	}

	public async Task Save(UserSettings userSettings, string userID)
	{
		var entity = userSettings.ToEntity(userID);
		var db = await _dbService.GetDb();

		await db.Settings.PutSafe(entity);
	}
}