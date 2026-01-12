
namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public interface ISpotifyPlaylistService
{
	Task AddTracks(string playlistId, List<string> trackUris, bool positionTop);
	void Cancel();
	Task CreatePlaylist(string name);
	Task LoadAndSync(bool forceUpdate = false);
	Task RemoveTracks(string playlistId, List<string> trackUris);
}