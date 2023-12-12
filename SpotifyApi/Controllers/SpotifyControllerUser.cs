using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerUser(ISpotifyApiClient client, IControllerApiUser controllerApiUser) : ISpotifyControllerUser
{
	private readonly ISpotifyApiClient _client = client;
	private readonly IControllerApiUser _controllerApiUser = controllerApiUser;

	private SpotifyUser? _user;

	public (Uri loginUrl, string loginVerifier) GetLoginUrl(Uri currentUrl)
	{
		var scope = new[]
		{
			Scopes.UserLibraryRead,
			Scopes.PlaylistReadPrivate,
			Scopes.PlaylistReadCollaborative,
			Scopes.UserFollowRead,
		};
		// TODO app id to config file
		const string appId = "67bbd538e581437597ae4574431682df";

		var (verifier, challenge) = PKCEUtil.GenerateCodes(120);
		var responseType = LoginRequest.ResponseType.Code;

		var loginRequest = new LoginRequest(currentUrl, appId, responseType)
		{
			Scope = scope,
			CodeChallengeMethod = "S256",
			CodeChallenge = challenge,
		};
		return (loginRequest.ToUri(), verifier);
	}

	public bool IsLoggedIn()
	{
		return _client.IsInicialized() && _user is not null;
	}

	public async Task<bool> LoginUser(string code, string loginVerifier, string redirectUrl)
	{
		_user = await _controllerApiUser.LoginUser(code, loginVerifier, redirectUrl);

		return _user is not null;
	}

	public async Task RefreshUser(SpotifyUser user)
	{
		var refreshToken = await _client.RefreshClient(user.Credentials!.RefreshToken!);

		user.Credentials.RefreshToken = refreshToken;
		_user = user;
	}

	public SpotifyUser? GetUser()
	{
		return _user;
	}
}
