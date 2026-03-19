using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Releases;

internal interface ISpotifyReleaseService
{
	Task Get(ReleaseGroup releaseGroup, bool forceUpdate = false);
}