using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Database.Spotify.Services
{
	public interface IDbSpotifyArtistService
	{
		Task<IReadOnlyList<SpotifyArtist>?> GetAll(CancellationToken ct);
		Task<IReadOnlyList<SpotifyArtist>> GetByIds(IEnumerable<string> ids, CancellationToken ct);
		Task Save(IReadOnlyList<SpotifyArtist> artists, CancellationToken ct);
	}
}