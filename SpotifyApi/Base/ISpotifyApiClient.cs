using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Base;

public interface ISpotifyApiClient
{
	ISpotifyClient GetClient();
	void SetClient(SpotifyClient spotifyClient);
	void Init(string accessToken);
	bool IsInicialized();
	Task<string?> RefreshClient(string refreshToken);
}