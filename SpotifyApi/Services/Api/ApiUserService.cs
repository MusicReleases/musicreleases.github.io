using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Services.Api;

internal class ApiUserService(ISpotifyApiClient client) : IApiUserService
{
	private readonly ISpotifyApiClient _client = client;

	public async Task<SpotifyUser?> LoginUser(string code, string loginVerifier, string redirectUrl)
	{
		// TODO app id to config file
		const string appId = "c63dcc19c74a4281b7edffe44b528680"; //"67bbd538e581437597ae4574431682df";

		var redirectUri = new Uri(redirectUrl);

		var tokenRequest = new PKCETokenRequest(appId, code, redirectUri, loginVerifier);

		var initialResponse = await new OAuthClient().RequestToken(tokenRequest);

		var authenticator = new PKCEAuthenticator(appId, initialResponse);

		var retryHandler = new SpotifyApiRetryHandler();

		var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(authenticator).WithRetryHandler(retryHandler);

		var spotifyClient = new SpotifyClient(config);

		_client.SetClient(spotifyClient);

		var userApi = await GetLoggedInUser();

		var user = GetUserFromApi(userApi, initialResponse.RefreshToken);
		return user;
	}

	private async Task<PrivateUser> GetLoggedInUser()
	{
		var spotifyClient = _client.GetClient();
		var user = await spotifyClient.UserProfile.Current();
		return user;
	}

	private SpotifyUser GetUserFromApi(PrivateUser userApi, string refreshToken)
	{
		var credentials = new SpotifyUserCredentials(refreshToken);
		var user = new SpotifyUser(userApi, credentials);
		return user;
	}

	public async Task<SpotifyUserInfo> GetLoggedInUserInfo()
	{
		var userApi = await GetLoggedInUser();
		var userInfo = new SpotifyUserInfo(userApi);
		return userInfo;
	}
}