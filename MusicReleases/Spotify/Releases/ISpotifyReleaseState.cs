using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Releases;

internal interface ISpotifyReleaseState : ISpotifyGroupedState<SpotifyRelease, ReleaseGroup>
{
}