using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Spotify.Releases
{
	public interface IDbSpotifyReleaseService
	{
		Task Add(SpotifyRelease release, CancellationToken ct);
		Task<IReadOnlyList<SpotifyRelease>> GetByIds(IEnumerable<string> ids, ReleaseEnums mainReleaseType, CancellationToken ct);
		Task Save(IReadOnlyList<SpotifyRelease> releases, CancellationToken ct);
	}
}