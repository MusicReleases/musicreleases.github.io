using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.BaseServices
{
	public interface ISpotifyFilterPlaylistService
	{
		IReadOnlyList<SpotifyPlaylist>? FilteredPlaylists { get; }
		string SearchText { get; }
		SpotifyEnums.PlaylistType TypeFilter { get; }

		event Action? OnFilterChanged;

		void Dispose();
		void SetSearchText(string text);
		void SetTypeFilter(SpotifyEnums.PlaylistType type);
	}
}