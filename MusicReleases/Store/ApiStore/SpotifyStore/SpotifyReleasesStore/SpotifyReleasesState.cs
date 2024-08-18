using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

public record SpotifyReleasesState : SpotifyObjectState<SpotifyRelease, SpotifyUserListUpdatePlaylists>
{
}
