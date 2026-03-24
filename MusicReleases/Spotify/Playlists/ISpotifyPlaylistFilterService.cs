using JakubKastner.SpotifyApi.Playlists;

namespace JakubKastner.MusicReleases.Spotify.Playlists;

internal interface ISpotifyPlaylistFilterService : IDisposable
{
	IReadOnlySet<SpotifyPlaylist>? FilteredPlaylists { get; }
	string? SearchText { get; }
	SpotifyPlaylistType PlaylistType { get; }

	//event Action? OnFilterChanged;
	//event Action? OnSearchTextChanged;
	event Action? OnChanged;

	bool SetSearchText(string? searchText);
	bool SetTypeFilter(SpotifyPlaylistType playlistType);
	IReadOnlySet<SpotifyPlaylist>? GetFilteredPlaylists(SpotifyPlaylistType playlistType, string? searchText);
}