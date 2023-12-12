using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public class ControllerApiUser(ISpotifyApiClient client) : IControllerApiUser
{
	private readonly ISpotifyApiClient _client = client;

	/*public async Task<SpotifyUser?> LoginUser(string url)
	{
		return await LoginUser(new Uri(url));
	}*/

	public async Task<SpotifyUser?> LoginUser(string code, string loginVerifier, string redirectUrl)
	{
		// TODO app id to config file
		const string appId = "67bbd538e581437597ae4574431682df";
		var redirectUri = new Uri(redirectUrl);

		var tokenRequest = new PKCETokenRequest(appId, code, redirectUri, loginVerifier);

		var initialResponse = await new OAuthClient().RequestToken(tokenRequest);

		var authenticator = new PKCEAuthenticator(appId, initialResponse);

		var config = SpotifyClientConfig.CreateDefault().WithAuthenticator(authenticator);
		var spotifyClient = new SpotifyClient(config);

		_client.SetClient(spotifyClient);

		var userApi = await GetLoggedInUser();


		/*var currentDate = DateTime.Now;

		// get url parameters
		var urlParameters = GetUrlParameters(url);

		// get user from access token
		var loggedIn = urlParameters.ContainsKey("access_token");
		if (!loggedIn)
		{
			return null;
		}

		var accessToken = urlParameters["access_token"];
		if (string.IsNullOrEmpty(accessToken))
		{
			return null;
		}
		_client.Init(accessToken);

		if (!urlParameters.ContainsKey("expires_in"))
		{
			return null;
		}

		// access token expiration
		var accessExpiration = Convert.ToInt32(urlParameters["expires_in"]);
		var accessExpirationDate = currentDate.AddSeconds(accessExpiration);

		// get user info
		var userApi = await GetLoggedInUser();
		//_user.Id = userApi.Id;*/

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

	private Dictionary<string, string> GetUrlParameters(Uri url)
	{
		var maxLen = Math.Min(1, url.Fragment.Length);
		var urlParameters = url.Fragment[maxLen..]?
		  .Split("&", StringSplitOptions.RemoveEmptyEntries)?
		  .Select(param => param.Split("=", StringSplitOptions.RemoveEmptyEntries))?
		  .ToDictionary(param => param[0], param => param[1]) ?? [];

		return urlParameters;
	}
}