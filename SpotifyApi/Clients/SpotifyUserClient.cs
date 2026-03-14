using JakubKastner.SpotifyApi.Store;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;

namespace JakubKastner.SpotifyApi.Clients;

internal class SpotifyUserClient(ISpotifyApiClient client, ISpotifyUserStore userStore, IRetryHandler retryHandler, SpotifyConfig spotifyConfig) : ISpotifyUserClient
{
	private readonly ISpotifyApiClient _client = client;
	private readonly ISpotifyUserStore _userStore = userStore;
	private readonly IRetryHandler _retryHandler = retryHandler;
	private readonly SpotifyConfig _spotifyConfig = spotifyConfig;

	public (Uri loginUrl, string loginVerifier) GetLoginUrl(string clientId, Uri currentUrl)
	{
		var scope = new[]
		{
			Scopes.UserLibraryRead,
			Scopes.PlaylistReadPrivate,
			Scopes.PlaylistReadCollaborative,
			Scopes.PlaylistModifyPrivate,
			Scopes.PlaylistModifyPublic,
			Scopes.UserFollowRead,
			Scopes.UserReadPrivate,
		};

		var (verifier, challenge) = PKCEUtil.GenerateCodes(120);

		var loginRequest = new LoginRequest(currentUrl, clientId, LoginRequest.ResponseType.Code)
		{
			Scope = scope,
			CodeChallengeMethod = "S256",
			CodeChallenge = challenge,
		};

		return (loginRequest.ToUri(), verifier);
	}

	public bool IsLoggedIn()
	{
		return _client.IsInitialized() && _userStore.UserIsNotNull();
	}

	public async Task<bool> LoginUser(string clientId, string code, string loginVerifier, string redirectUrl)
	{
		var redirectUri = new Uri(redirectUrl);

		var request = new PKCETokenRequest(clientId, code, redirectUri, loginVerifier);
		var response = await new OAuthClient().RequestToken(request);

		var authenticator = new PKCEAuthenticator(clientId, response);

		var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(authenticator).WithRetryHandler(_retryHandler);

		SetClient(config);

		var userApi = await GetUserInfoFromApi();

		_userStore.SetUser(userApi, response.RefreshToken);

		return _userStore.UserIsNotNull();
	}

	public async Task<string> RefreshAccessToken()
	{
		await RefreshAccessTokenInternal();

		var user = GetUserRequired();
		return user.Credentials.RefreshToken;
	}

	public async Task SetUserFromDb(SpotifyUser user)
	{
		_userStore.SetUser(user);
		await RefreshAccessTokenInternal();
	}

	private async Task RefreshAccessTokenInternal()
	{
		var user = GetUserRequired();
		try
		{
			var request = new PKCETokenRefreshRequest(_spotifyConfig.ClientId, user.Credentials.RefreshToken);
			var response = await new OAuthClient().RequestToken(request);

			_userStore.SetRefreshToken(response.RefreshToken);

			var config = SpotifyClientConfig.CreateDefault(response.AccessToken).WithRetryHandler(_retryHandler);
			SetClient(config);

			var lastUpdate = DateTime.Now - user.Info.LastUpdate;
			if (lastUpdate.Days > 0)
			{
				// update userinfo every day
				var userApi = await GetUserInfoFromApi();

				_userStore.SetUserInfoApi(userApi);
			}
		}
		catch (Exception)
		{
			_userStore.DeleteUser();
		}
	}

	private async Task<PrivateUser> GetUserInfoFromApi()
	{
		var userApi = await _client.GetClient().UserProfile.Current();
		return userApi;
	}

	private void SetClient(SpotifyClientConfig config)
	{
		var spotifyClient = new SpotifyClient(config);
		_client.SetClient(spotifyClient);
	}

	public SpotifyUser? GetUser() => _userStore.GetUser();
	public SpotifyUser GetUserRequired() => _userStore.GetUserRequired();
	public string GetUserIdRequired() => _userStore.GetUserIdRequired();
}