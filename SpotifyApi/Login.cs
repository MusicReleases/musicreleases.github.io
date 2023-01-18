using SpotifyAPI.Web;
using ControllerUser = JakubKastner.SpotifyApi.Controllers.Api.ControllerApiUser;

namespace JakubKastner.SpotifyApi;

public class Login
{
	private readonly ControllerUser _controllerUser;

	public Login(ControllerUser controllerUser)
	{
		_controllerUser = controllerUser;
	}

	public static Uri GetLoginUrl(Uri currentUrl)
	{
		ICollection<string>? scope = new[] { Scopes.UserLibraryRead, Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative, Scopes.UserFollowRead };
		const string appId = "67bbd538e581437597ae4574431682df";

		var loginRequest = new LoginRequest(currentUrl, appId, LoginRequest.ResponseType.Token)
		{
			Scope = scope
		};
		return loginRequest.ToUri();
	}

	public async Task LoginUser(string url)
	{
		await LoginUser(new Uri(url));
	}

	public async Task LoginUser(Uri url)
	{
		_controllerUser.LoginUser(url);
	}
}
