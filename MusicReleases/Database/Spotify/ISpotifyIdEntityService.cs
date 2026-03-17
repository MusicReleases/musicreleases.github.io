using JakubKastner.MusicReleases.Spotify;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Database.Spotify;

internal interface ISpotifyIdEntityService<TModel, TPayload> where TModel : SpotifyIdNameObject where TPayload : ISpotifyPayload
{
	Task<IReadOnlyCollection<TModel>> GetByIds(IReadOnlyCollection<TPayload> payloads, CancellationToken ct);
	Task Save(IReadOnlyCollection<TModel> items, CancellationToken ct);
	Task Save(TModel item, CancellationToken ct);
}