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
		_spotifyClient = new SpotifyClient(accessToken);
	}

	public void SetClient(SpotifyClient spotifyClient)
	{
		_spotifyClient = spotifyClient;
	}

	public bool IsInicialized()
	{
		return _spotifyClient != null;
	}

	public ISpotifyClient GetClient()
	{
		if (!IsInicialized())
		{
			throw new NullReferenceException(nameof(ISpotifyClient));
		}
		return _spotifyClient!;
	}

	public async Task<string> RefreshClient(string refreshToken)
	{
		// TODO app id to config file
		const string appId = "67bbd538e581437597ae4574431682df";
		var refreshRequest = new PKCETokenRefreshRequest(appId, refreshToken);
		var newResponse = await new OAuthClient().RequestToken(refreshRequest);

		var client = new SpotifyClient(newResponse.AccessToken);
		SetClient(client);

		return newResponse.RefreshToken;
	}
}
