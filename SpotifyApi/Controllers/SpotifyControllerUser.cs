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
			Scopes.UserReadPrivate, // user country
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
		if (string.IsNullOrEmpty(refreshToken))
		{
			_user = null;
			return;
		}
		user.Credentials.RefreshToken = refreshToken;

		var lastUpdate = DateTime.Now - user.Info!.LastUpdate;
		if (lastUpdate.Days > 0)
		{
			// update userinfo every day
			user.Info = await _controllerApiUser.GetLoggedInUserInfo();
		}

		_user = user;
	}

	public SpotifyUser? GetUser()
	{
		return _user;
	}

	public SpotifyUser GetUserRequired()
	{
		if (_user is null)
		{
			throw new UnauthorizedAccessException(nameof(GetUserRequired));
		}

		return _user;
	}

	public string GetUserIdRequired()
	{
		var user = GetUserRequired();
		var userId = user.Info?.Id;
		if (string.IsNullOrEmpty(userId))
		{
			throw new UnauthorizedAccessException(nameof(GetUserRequired));
		}

		return userId;
	}
}
