using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public interface IControllerApiUser
{
	Task<SpotifyUser?> LoginUser(string code, string loginVerifier, string redirectUrl);
	//Task<SpotifyUser?> LoginUser(Uri url);
}