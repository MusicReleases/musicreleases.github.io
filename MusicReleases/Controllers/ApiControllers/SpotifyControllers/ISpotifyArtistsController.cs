namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public interface ISpotifyArtistsController
{
	void GetArtists(bool forceUpdate = false);
}