using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Services.Api
{
	public interface IApiReleaseClient
	{
		Task<List<SpotifyRelease>> GetByArtist(SpotifyArtist artist, ReleaseGroup releaseType, CancellationToken ct = default);
		Task<List<SpotifyRelease>> GetByArtists(IEnumerable<SpotifyArtist> artistIds, ReleaseGroup releaseType, CancellationToken ct = default);
	}
}