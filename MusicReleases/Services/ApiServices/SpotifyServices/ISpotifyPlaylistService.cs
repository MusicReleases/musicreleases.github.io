using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices
{
	public interface ISpotifyPlaylistService
	{
		Task AddTracks(string playlistId, IEnumerable<SpotifyTrack> tracks, bool positionTop);
		void Cancel();
		Task CreatePlaylist(string name);
		Task LoadAndSync(bool forceUpdate = false);
		Task RemoveTracks(string playlistId, IEnumerable<SpotifyTrack> tracks);
	}
}