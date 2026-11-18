namespace JakubKastner.SpotifyApi.User;

public interface ISpotifyUserClient
{
	(Uri loginUrl, string loginVerifier) GetLoginUrl(string clientId, string redirectUrl);
	SpotifyUser? GetUser();
	string GetUserIdRequired();
	SpotifyUser GetUserRequired();
	bool IsLoggedIn();
	Task<bool> LoginUser(string clientId, string code, string loginVerifier, string redirectUrl);
	Task SetUserFromDb(SpotifyUser user);
}