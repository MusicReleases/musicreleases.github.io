using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public interface IControllerApiUser
{
	Task<PrivateUser> GetLoggedInUser();
	SpotifyUser GetUser(PrivateUser userApi);
	Task<SpotifyUser?> LoginUser(string url);
	Task<SpotifyUser?> LoginUser(Uri url);
}