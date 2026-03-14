using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Clients
{
	public interface ISpotifyReleaseClient
	{
		Task<List<SpotifyRelease>> GetByArtist(SpotifyArtist artist, ReleaseGroup releaseType, CancellationToken ct = default);
		Task<List<SpotifyRelease>> GetByArtists(IEnumerable<SpotifyArtist> artistIds, ReleaseGroup releaseType, CancellationToken ct = default);
	}
}