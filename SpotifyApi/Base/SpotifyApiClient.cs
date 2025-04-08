using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Base;

public class SpotifyApiClient : ISpotifyApiClient
{
	private ISpotifyClient? _spotifyClient;

	public void Init(string accessToken)
	{
		if (string.IsNullOrEmpty(accessToken))
		{
			throw new ArgumentNullException(nameof(accessToken));
		}

		var retryHandler = new SpotifyApiRetryHandler();
		var config = SpotifyClientConfig.CreateDefault(accessToken).WithRetryHandler(retryHandler);

		var client = new SpotifyClient(config);
		SetClient(client);
	}

	public void SetClient(SpotifyClient spotifyClient)
	{
		_spotifyClient = spotifyClient;
	}

	public bool IsInicialized()
	{
		return _spotifyClient is not null;
	}

	public ISpotifyClient GetClient()
	{
		if (!IsInicialized())
		{
			throw new NullReferenceException(nameof(ISpotifyClient));
		}
		return _spotifyClient!;
	}

	public async Task<string?> RefreshClient(string refreshToken)
	{
		// TODO app id to config file
		const string appId = /*"c63dcc19c74a4281b7edffe44b528680";*/ "67bbd538e581437597ae4574431682df";
		var refreshRequest = new PKCETokenRefreshRequest(appId, refreshToken);
		PKCETokenResponse? newResponse;
		try
		{
			newResponse = await new OAuthClient().RequestToken(refreshRequest);

		}
		catch (Exception)
		{
			return null;
		}

		var client = new SpotifyClient(newResponse.AccessToken);
		SetClient(client);

		return newResponse.RefreshToken;
	}
}
