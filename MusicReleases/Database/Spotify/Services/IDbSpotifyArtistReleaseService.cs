using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Database.Spotify.Services
{
	public interface IDbSpotifyArtistReleaseService
	{
		Task AddArtistRelease(string artistId, string releaseId, ArtistReleaseRole artistRole);
		Task DeleteAllForArtist(string artistId);
		Task<HashSet<string>> GetArtistIds(string releaseId, ArtistReleaseRole artistRole);
		Task<HashSet<SpotifyArtistReleaseEntity>> GetByReleaseIds(IEnumerable<string> releaseIds);
		Task<HashSet<string>> GetReleaseIds(IEnumerable<string> artistIds, ArtistReleaseRole artistRole);
		Task<HashSet<string>> GetReleaseIds(string artistId, ArtistReleaseRole artistRole);
		Task Save(IEnumerable<SpotifyArtistReleaseEntity> links);
		Task SetArtistReleases(string artistId, ReleaseGroup mainReleaseType, IEnumerable<string> releaseApiIdsEnumerable);
		Task SetArtistReleases(IEnumerable<SpotifyRelease> releasesWithArtists, ArtistReleaseRole artistRole);
	}
}