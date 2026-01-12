
namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyUserPlaylistService
{
	Task AddUserPlaylist(string userId, string playlistId, int order);
	Task DeleteAllForUser(string userId);
	Task<HashSet<string>> GetUserPlaylistIds(string userId);
	Task<Dictionary<string, int>> GetUserPlaylistOrder(string userId);
	Task SetUserPlaylists(string userId, IEnumerable<string> apiIdsEnumerable);
}