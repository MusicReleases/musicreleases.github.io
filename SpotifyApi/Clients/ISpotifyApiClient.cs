using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Clients;

public interface ISpotifyApiClient
{
	ISpotifyClient GetClient();
	void SetClient(ISpotifyClient spotifyClient);
	bool IsInicialized();
}