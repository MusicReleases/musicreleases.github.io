namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public interface ISpotifyLoginStorageService
{
	Task DeleteLoginVerifier();
	Task DeleteSavedUser();
	Task<string?> GetLoginVerifier();
	Task<string?> GetSavedUserId();
	Task SaveLoginVerifier(string loginVerifier);
	Task SaveUserId(string userId);
}