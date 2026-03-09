using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Database.Spotify.Services
{
	public interface IDbSpotifyPlaylistService
	{
		Task Add(SpotifyPlaylist playlist);
		Task<IReadOnlyList<SpotifyPlaylist>?> GetAll();
		Task<IReadOnlyList<SpotifyPlaylist>> GetByIds(IEnumerable<string> ids);
		Task Save(IReadOnlyList<SpotifyPlaylist> playlists);
		Task UpdateSnapshot(string playlistId, string newSnapshotId);
	}
}