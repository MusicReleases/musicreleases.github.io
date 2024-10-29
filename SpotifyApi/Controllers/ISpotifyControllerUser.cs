using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public interface ISpotifyControllerUser
{
	(Uri loginUrl, string loginVerifier) GetLoginUrl(Uri currentUrl);
	bool IsLoggedIn();
	Task<bool> LoginUser(string code, string loginVerifier, string redirectUrl);
	SpotifyUser? GetUser();
	SpotifyUser GetUserRequired();
	string GetUserIdRequired();
	Task RefreshUser(SpotifyUser user);
}