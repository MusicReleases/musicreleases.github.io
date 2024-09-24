
namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers
{
	public interface ISpotifyLoginStorageController
	{
		Task DeleteLoginVerifier();
		Task DeleteSavedUser();
		Task<string?> GetLoginVerifier();
		Task<string?> GetSavedUserId();
		Task SaveLoginVerifier(string loginVerifier);
		Task SaveUserId(string userId);
	}
}