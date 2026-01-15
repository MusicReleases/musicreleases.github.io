using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.BaseServices;

public interface ISpotifyFilterPlaylistService
{
	IReadOnlyList<SpotifyPlaylist>? FilteredPlaylists { get; }
	string SearchText { get; }
	PlaylistType TypeFilter { get; }

	event Action? OnFilterChanged;

	void Dispose();
	void SetSearchText(string text);
	void SetTypeFilter(PlaylistType type);
}