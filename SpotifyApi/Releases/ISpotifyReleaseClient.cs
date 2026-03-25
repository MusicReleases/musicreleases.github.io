using JakubKastner.SpotifyApi.Artists;

namespace JakubKastner.SpotifyApi.Releases;

public interface ISpotifyReleaseClient
{
	Task<List<SpotifyRelease>> GetByArtist(SpotifyArtist artist, ReleaseGroup releaseGroup, CancellationToken ct = default);
	Task<List<SpotifyRelease>> GetByArtist(SpotifyArtist artist, ReleaseGroup releaseGroup, DateTime cutoff, CancellationToken ct = default);
	Task<List<SpotifyRelease>> GetByArtists(IEnumerable<SpotifyArtist> artistIds, ReleaseGroup releaseType, CancellationToken ct = default);
}