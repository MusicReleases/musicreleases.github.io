namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public interface ISpotifyPlaylistsController
{
	void GetPlaylists(bool forceUpdate = false);
}