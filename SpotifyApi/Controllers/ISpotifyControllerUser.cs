namespace JakubKastner.SpotifyApi.Controllers;

public interface ISpotifyControllerUser
{
	Uri GetLoginUrl(Uri currentUrl);
	bool IsLoggedIn();
	Task<bool> LoginUser(string currentUrl);
}