using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;

namespace JakubKastner.MusicReleases.Database.Spotify;

internal interface ISpotifyArtistLinkEntityService<TArtistLinkEntity> : ISpotifyLinkEntityServiceCore where TArtistLinkEntity : ISpotifyDb, ISpotifyArtistLinkEntity
{
	Task Save(IReadOnlyCollection<TArtistLinkEntity> entities, CancellationToken ct);
}