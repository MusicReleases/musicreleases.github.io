using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;

namespace JakubKastner.MusicReleases.Database.Spotify;

internal interface ISpotifyArtistLinkEntityService<TArtistLinkEntity> : ISpotifyLinkEntityService where TArtistLinkEntity : ISpotifyDb, ISpotifyArtistLinkEntity
{
	Task<IEnumerable<TArtistLinkEntity>> FetchByKeys2(IEnumerable<string> releaseIds);
	Task Save(IReadOnlyCollection<TArtistLinkEntity> entities, CancellationToken ct);
}