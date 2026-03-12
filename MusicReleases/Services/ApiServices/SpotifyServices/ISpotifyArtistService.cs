
namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public interface ISpotifyArtistService
{
	Task Get(bool forceUpdate = false);
}