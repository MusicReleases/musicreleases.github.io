using JakubKastner.MusicReleases.Objects.User;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices
{
	public interface IDbSpotifyUserSettingsService
	{
		Task<UserSettings> Get(string userId);
		Task Save(UserSettings userSettings, string userID);
	}
}