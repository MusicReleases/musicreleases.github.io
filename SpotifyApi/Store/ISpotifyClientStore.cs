using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Store;

internal interface ISpotifyClientStore
{
	ISpotifyClient GetClient();
	void SetClient(ISpotifyClient spotifyClient);
	bool IsInitialized();
}