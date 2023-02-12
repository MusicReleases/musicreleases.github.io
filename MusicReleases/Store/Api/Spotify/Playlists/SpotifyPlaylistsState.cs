using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.Releases;

public record SpotifyPlaylistsState
{
	public bool Initialized { get; init; }
	public bool Loading { get; init; }
	public HashSet<Playlist>? Playlists { get; init; }
}
