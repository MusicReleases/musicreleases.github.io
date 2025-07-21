using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Services.Api;

internal interface IApiUserService
{
	Task<SpotifyUser?> LoginUser(string clientId, string code, string loginVerifier, string redirectUrl);
	Task<SpotifyUserInfo> GetLoggedInUserInfo();
	//Task<SpotifyUser?> LoginUser(Uri url);
}