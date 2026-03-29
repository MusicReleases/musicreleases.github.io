using JakubKastner.MusicReleases.Spotify.Tasks;
using JakubKastner.SpotifyApi.Tracks;

namespace JakubKastner.MusicReleases.Spotify.Playlists;

internal interface ISpotifyPlaylistService
{
	Task AddTrack(string playlistId, SpotifyTrack track, bool positionTop);
	Task AddTracks(string playlistId, IEnumerable<SpotifyTrack> tracks, bool positionTop);
	Task CreatePlaylist(string name);
	Task Get(bool forceUpdate = false);
	Task GetInTask(BackgroundTask task, bool forceUpdate = false);
	Task RemoveTrack(string playlistId, SpotifyTrack track);
	Task RemoveTracks(string playlistId, IEnumerable<SpotifyTrack> tracks);
}