using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.SpotifyServices;

public interface ISpotifyFilterPlaylistService
{
	IReadOnlyList<SpotifyPlaylist>? FilteredPlaylists { get; }
	string SearchText { get; }
	PlaylistType FilterType { get; }

	event Action? OnFilterChanged;

	void Dispose();
	void SetSearchText(string text);
	void SetTypeFilter(PlaylistType type);
	IEnumerable<SpotifyPlaylist>? GetFilteredPlaylists(string searchText, PlaylistType typeFilter);
}