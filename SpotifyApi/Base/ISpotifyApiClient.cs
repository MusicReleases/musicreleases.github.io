using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Base;

public interface ISpotifyApiClient
{
	ISpotifyClient GetClient();
	void Init(string accessToken);
	bool IsInicialized();
}