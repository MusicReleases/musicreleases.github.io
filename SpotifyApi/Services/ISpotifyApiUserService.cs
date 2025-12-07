using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Services;

public interface ISpotifyApiUserService
{
	(Uri loginUrl, string loginVerifier) GetLoginUrl(string clientId, Uri currentUrl);
	bool IsLoggedIn();
	Task<bool> LoginUser(string clientId, string code, string loginVerifier, string redirectUrl);
	SpotifyUser? GetUser();
	SpotifyUser GetUserRequired();
	string GetUserIdRequired();
	Task RefreshUser(SpotifyUser user);
}