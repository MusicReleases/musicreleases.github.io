using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Base;

public class SpotifyApiClient(SpotifyConfig spotifyConfig) : ISpotifyApiClient
{
	private ISpotifyClient? _spotifyClient;
	private readonly SpotifyConfig _spotifyConfig = spotifyConfig;

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
		var refreshRequest = new PKCETokenRefreshRequest(_spotifyConfig.ClientId, refreshToken);
		PKCETokenResponse? newResponse;
		try
		{
			newResponse = await new OAuthClient().RequestToken(refreshRequest);

		}
		catch (Exception)
		{
			return null;
		}

		Init(newResponse.AccessToken);
		return newResponse.RefreshToken;
	}
}
