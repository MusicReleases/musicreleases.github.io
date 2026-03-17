using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Playlists;

namespace JakubKastner.MusicReleases.Spotify.Playlists;

internal interface ISpotifyPlaylistFilterService : IDisposable
{
	IReadOnlySet<SpotifyPlaylist>? FilteredPlaylists { get; }
	string? SearchText { get; }
	PlaylistEnums PlaylistType { get; }

	//event Action? OnFilterChanged;
	//event Action? OnSearchTextChanged;
	event Action? OnChanged;

	void SetSearchText(string? searchText);
	void SetTypeFilter(PlaylistEnums playlistType);
	IReadOnlySet<SpotifyPlaylist>? GetFilteredPlaylists(PlaylistEnums playlistType, string? searchText);
}