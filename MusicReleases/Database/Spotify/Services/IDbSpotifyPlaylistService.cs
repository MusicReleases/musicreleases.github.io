using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Database.Spotify.Services
{
	public interface IDbSpotifyPlaylistService
	{
		Task Add(SpotifyPlaylist playlist, CancellationToken ct);
		Task<IReadOnlyList<SpotifyPlaylist>?> GetAll(CancellationToken ct);
		Task<IReadOnlyList<SpotifyPlaylist>> GetByIds(IEnumerable<string> ids, CancellationToken ct);
		Task Save(IReadOnlyList<SpotifyPlaylist> playlists, CancellationToken ct);
		Task UpdateSnapshot(string playlistId, string newSnapshotId, CancellationToken ct);
	}
}