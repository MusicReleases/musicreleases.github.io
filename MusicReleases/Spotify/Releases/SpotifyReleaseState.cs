using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Releases;

internal sealed class SpotifyReleaseState : SpotifyGroupedState<SpotifyRelease, ReleaseGroup>, ISpotifyReleaseState
{
}