using JakubKastner.SpotifyApi.Enums;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Database.Spotify.Services
{
	public interface IDbSpotifyReleaseService
	{
		Task Add(SpotifyRelease release, CancellationToken ct);
		Task<IReadOnlyList<SpotifyRelease>> GetByIds(IEnumerable<string> ids, ReleaseEnums mainReleaseType, CancellationToken ct);
		Task Save(IReadOnlyList<SpotifyRelease> releases, CancellationToken ct);
	}
}