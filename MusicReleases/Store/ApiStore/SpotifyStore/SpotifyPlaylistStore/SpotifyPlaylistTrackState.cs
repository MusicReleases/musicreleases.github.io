using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;

public record SpotifyPlaylistTrackState : SpotifyObjectState<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>
{
}