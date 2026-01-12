using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.State.Spotify;

public interface ISpotifyPlaylistState
{
	IReadOnlyList<SpotifyPlaylist>? Playlists { get; }

	event Action? OnChange;

	void Add(SpotifyPlaylist playlist);
	SpotifyPlaylist? GetById(string id);
	void SetPlaylists(IEnumerable<SpotifyPlaylist> playlists);
}