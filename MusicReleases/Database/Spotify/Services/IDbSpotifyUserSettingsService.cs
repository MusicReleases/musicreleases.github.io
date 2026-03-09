using JakubKastner.MusicReleases.Objects.User;

namespace JakubKastner.MusicReleases.Database.Spotify.Services
{
	public interface IDbSpotifyUserSettingsService
	{
		Task<UserSettings?> Get(string userId);
		Task Save(UserSettings userSettings, string userID);
	}
}