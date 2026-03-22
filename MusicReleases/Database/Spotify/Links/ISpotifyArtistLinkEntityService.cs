using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.MusicReleases.Database.Spotify.LinkEntities;

namespace JakubKastner.MusicReleases.Database.Spotify.Links;

internal interface ISpotifyArtistLinkEntityService<TArtistLinkEntity> : ISpotifyLinkEntityServiceCore where TArtistLinkEntity : ISpotifyDb, ISpotifyArtistLinkEntity
{
	Task Save(IReadOnlyCollection<TArtistLinkEntity> entities, CancellationToken ct);
}