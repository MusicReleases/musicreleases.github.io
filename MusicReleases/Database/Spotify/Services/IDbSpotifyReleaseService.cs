using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Database.Spotify.Services
{
	public interface IDbSpotifyReleaseService
	{
		Task Add(SpotifyRelease release);
		Task<IReadOnlyList<SpotifyRelease>> GetByIds(IEnumerable<string> ids, MainReleasesType mainReleaseType);
		Task Save(IReadOnlyList<SpotifyRelease> releases);
	}
}