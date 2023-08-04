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
}
