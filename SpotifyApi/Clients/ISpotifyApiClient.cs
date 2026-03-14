using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Clients;

internal interface ISpotifyApiClient
{
	ISpotifyClient GetClient();
	void SetClient(ISpotifyClient spotifyClient);
	bool IsInitialized();
}