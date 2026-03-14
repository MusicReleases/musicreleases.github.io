using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.SpotifyServices;

public interface ISpotifyPlaylistFilterService
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