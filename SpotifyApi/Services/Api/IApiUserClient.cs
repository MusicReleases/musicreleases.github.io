using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Services.Api
{
	public interface IApiUserClient
	{
		(Uri loginUrl, string loginVerifier) GetLoginUrl(string clientId, Uri currentUrl);
		SpotifyUser? GetUser();
		string GetUserIdRequired();
		SpotifyUser GetUserRequired();
		bool IsLoggedIn();
		Task<bool> LoginUser(string clientId, string code, string loginVerifier, string redirectUrl);
		Task<string> RefreshAccessToken();
		Task SetUserFromDb(SpotifyUser user);
	}
}