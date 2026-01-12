
namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public interface ISpotifyArtistService
{
	void Cancel();
	Task Get(bool forceUpdate = false);
}