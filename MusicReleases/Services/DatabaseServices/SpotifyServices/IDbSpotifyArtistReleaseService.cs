using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices
{
	public interface IDbSpotifyArtistReleaseService
	{
		Task AddArtistRelease(string artistId, string releaseId, ArtistReleaseRole artistRole);
		Task DeleteAllForArtist(string artistId);
		Task<HashSet<string>> GetArtistIds(string releaseId, ArtistReleaseRole artistRole);
		Task<HashSet<string>> GetReleaseIds(IEnumerable<string> artistIds, ArtistReleaseRole artistRole);
		Task<HashSet<string>> GetReleaseIds(string artistId, ArtistReleaseRole artistRole);
		Task SetArtistReleases(string artistId, MainReleasesType mainReleaseType, IEnumerable<string> releaseApiIdsEnumerable);
		Task SetArtistReleases(IEnumerable<SpotifyRelease> releasesWithArtists, ArtistReleaseRole artistRole);
	}
}