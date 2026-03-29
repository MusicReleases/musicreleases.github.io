using JakubKastner.MusicReleases.Spotify.Tasks;

namespace JakubKastner.MusicReleases.Spotify.Artists;

internal interface ISpotifyArtistService
{
	Task Get(bool forceUpdate = false);
	Task GetInTask(BackgroundTask task, bool forceUpdate = false);
}