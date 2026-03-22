using JakubKastner.MusicReleases.Spotify;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Database.Spotify.IdEntities;

internal interface ISpotifyIdEntityService<TModel> : ISpotifyIdEntityStoreService<TModel>
	where TModel : SpotifyIdNameObject
{
	Task<TModel?> GetById(string id, CancellationToken ct);
	Task<IReadOnlyCollection<TModel>> GetByIds(IReadOnlyCollection<string> ids, CancellationToken ct);
}

internal interface ISpotifyIdEntityService<TModel, TPayload> : ISpotifyIdEntityStoreService<TModel>
	where TModel : SpotifyIdNameObject
	where TPayload : ISpotifyPayload
{
	Task<TModel?> GetById(TPayload payload, CancellationToken ct);
	Task<IReadOnlyCollection<TModel>> GetByIds(IReadOnlyCollection<TPayload> payloads, CancellationToken ct);
}