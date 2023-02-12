using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Playlists;

public record SpotifyPlaylistsState
{
	public bool Initialized { get; init; }
	public bool Loading { get; init; }
	public SortedSet<Playlist>? Playlists { get; init; }
}
