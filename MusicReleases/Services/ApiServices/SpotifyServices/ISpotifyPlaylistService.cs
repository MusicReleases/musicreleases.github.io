using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public interface ISpotifyPlaylistService
{
	Task AddTrack(string playlistId, SpotifyTrack track, bool positionTop);
	Task AddTracks(string playlistId, IEnumerable<SpotifyTrack> tracks, bool positionTop);
	void Cancel();
	Task CreatePlaylist(string name);
	Task LoadAndSync(bool forceUpdate = false);
	Task RemoveTrack(string playlistId, SpotifyTrack track);
	Task RemoveTracks(string playlistId, IEnumerable<SpotifyTrack> tracks);
}