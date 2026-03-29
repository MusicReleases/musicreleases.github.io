using JakubKastner.MusicReleases.Spotify.Tasks;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Releases;

internal interface ISpotifyReleaseService
{
	Task Get(ReleaseGroup releaseGroup, bool forceUpdate = false);
	Task GetInTask(BackgroundTask task, ReleaseGroup releaseType, bool forceUpdate = false);
}