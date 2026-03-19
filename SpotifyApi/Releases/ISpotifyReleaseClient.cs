using JakubKastner.SpotifyApi.Artists;

namespace JakubKastner.SpotifyApi.Releases;

public interface ISpotifyReleaseClient
{
	Task<List<SpotifyRelease>> GetByArtist(SpotifyArtist artist, ReleaseGroup releaseType, CancellationToken ct = default);
	Task<List<SpotifyRelease>> GetByArtists(IEnumerable<SpotifyArtist> artistIds, ReleaseGroup releaseType, CancellationToken ct = default);
}