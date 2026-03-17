using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Playlists;

namespace JakubKastner.MusicReleases.Spotify.Playlists;

internal interface ISpotifyPlaylistFilterService
{
	IReadOnlyList<SpotifyPlaylist>? FilteredPlaylists { get; }
	string SearchText { get; }
	PlaylistEnums FilterType { get; }

	event Action? OnFilterChanged;

	void Dispose();
	void SetSearchText(string text);
	void SetTypeFilter(PlaylistEnums type);
	IEnumerable<SpotifyPlaylist>? GetFilteredPlaylists(string searchText, PlaylistEnums typeFilter);
}