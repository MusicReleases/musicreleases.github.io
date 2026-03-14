using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.SpotifyApi.Enums;

namespace JakubKastner.MusicReleases.Spotify.Artists.Releases;

internal interface ISpotifyArtistReleaseDbService
{
	//Task AddArtistRelease(string artistId, string releaseId, ArtistReleaseRole artistRole, CancellationToken ct);
	//Task DeleteAllForArtist(string artistId, CancellationToken ct);
	//Task<HashSet<string>> GetArtistIds(string releaseId, ArtistReleaseRole artistRole, CancellationToken ct);
	Task<HashSet<SpotifyArtistReleaseEntity>> GetByReleaseIds(IEnumerable<string> releaseIds, CancellationToken ct);
	Task<HashSet<string>> GetReleaseIds(IEnumerable<string> artistIds, ArtistReleaseRole artistRole, CancellationToken ct);
	//Task<HashSet<string>> GetReleaseIds(string artistId, ArtistReleaseRole artistRole, CancellationToken ct);
	Task Save(IEnumerable<SpotifyArtistReleaseEntity> links, CancellationToken ct);
	//Task SetArtistReleases(string artistId, ReleaseEnums mainReleaseType, IEnumerable<string> releaseApiIdsEnumerable, CancellationToken ct);
	//Task SetArtistReleases(IEnumerable<SpotifyRelease> releasesWithArtists, ArtistReleaseRole artistRole, CancellationToken ct);
}