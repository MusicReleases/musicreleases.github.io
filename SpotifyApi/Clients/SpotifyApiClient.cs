using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Clients;

public class SpotifyApiClient : ISpotifyApiClient
{
	private ISpotifyClient? _spotifyClient;

	public void SetClient(ISpotifyClient spotifyClient)
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
}
