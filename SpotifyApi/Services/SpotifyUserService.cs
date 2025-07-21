﻿using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services.Api;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Services;

internal class SpotifyUserService(ISpotifyApiClient client, IApiUserService controllerApiUser) : ISpotifyUserService
{
	private readonly ISpotifyApiClient _client = client;
	private readonly IApiUserService _controllerApiUser = controllerApiUser;

	private SpotifyUser? _user;

	public (Uri loginUrl, string loginVerifier) GetLoginUrl(string clientId, Uri currentUrl)
	{
		var scope = new[]
		{
			Scopes.UserLibraryRead,
			Scopes.PlaylistReadPrivate,
			Scopes.PlaylistReadCollaborative,
			Scopes.UserFollowRead,
			Scopes.UserReadPrivate, // user country
		};

		var (verifier, challenge) = PKCEUtil.GenerateCodes(120);
		var responseType = LoginRequest.ResponseType.Code;

		var loginRequest = new LoginRequest(currentUrl, clientId, responseType)
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

	public async Task<bool> LoginUser(string clientId, string code, string loginVerifier, string redirectUrl)
	{
		_user = await _controllerApiUser.LoginUser(clientId, code, loginVerifier, redirectUrl);

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
