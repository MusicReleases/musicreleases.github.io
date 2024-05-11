using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

public record SpotifyPlaylistsState
{
	public bool Initialized { get; init; }
	public bool Loading { get; init; }
	public SpotifyUserList<SpotifyPlaylist>? Playlists { get; init; }
}
