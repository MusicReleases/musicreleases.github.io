using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.SpotifyApi.Releases;

namespace JakubKastner.MusicReleases.Database.Spotify;

internal interface ISpotifyArtistLinkService<TArtistLinkEntity> : ISpotifyLinkEntityService where TArtistLinkEntity : ISpotifyDb, ISpotifyArtistLinkEntity
{
	//Task<IReadOnlyCollection<SpotifyArtistGroupPayload>> GetArtistsByArtistIds(IReadOnlyCollection<string> artistIds, CancellationToken ct);
	//Task<Dictionary<string, IReadOnlyCollection<string>>> GetReleasesByArtistIds(IReadOnlyCollection<string> artistIds, CancellationToken ct);
	//Task<object> GetArtistsByReleaseIds(Dictionary<string, IReadOnlyCollection<string>> releaseIds, CancellationToken ct);
	Task Save(IReadOnlyCollection<TArtistLinkEntity> entities, CancellationToken ct);
	Task<IReadOnlyCollection<SpotifyArtistGroupPayload>> GetArtistsByArtistIds(IReadOnlyCollection<string> artistIds, ReleaseGroup releaseGroup, CancellationToken ct);
}