using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Base;

public interface ISpotifyApiClient
{
	ISpotifyClient GetClient();
	void SetClient(SpotifyClient spotifyClient);
	bool IsInicialized();
}