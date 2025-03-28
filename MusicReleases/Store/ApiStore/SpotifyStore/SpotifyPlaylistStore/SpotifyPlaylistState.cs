using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistStore;

public record SpotifyPlaylistState : SpotifyObjectState<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>
{
}