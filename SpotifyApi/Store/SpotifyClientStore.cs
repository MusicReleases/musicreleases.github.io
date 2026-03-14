using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Store;

internal class SpotifyClientStore : ISpotifyClientStore
{
	private ISpotifyClient? _spotifyClient;

	public void SetClient(ISpotifyClient spotifyClient)
	{
		_spotifyClient = spotifyClient;
	}

	public bool IsInitialized()
	{
		return _spotifyClient is not null;
	}

	public ISpotifyClient GetClient()
	{
		if (!IsInitialized())
		{
			throw new InvalidOperationException($"{nameof(ISpotifyClient)} is not initialized.");
		}
		return _spotifyClient!;
	}
}
