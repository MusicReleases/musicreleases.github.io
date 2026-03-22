using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.MusicReleases.Spotify;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Database.Spotify.IdEntities;

internal abstract class SpotifyIdEntityService<TModel, TIdEntity> : SpotifyIdEntityStoreService<TModel, TIdEntity>, ISpotifyIdEntityService<TModel> where TModel : SpotifyIdNameObject
	where TIdEntity : ISpotifyDb, ISpotifyIdEntity
{
	protected abstract TModel ToModel(TIdEntity entity);

	public async Task<TModel?> GetById(string id, CancellationToken ct)
	{
		var entity = await GetEntityByIdCore(id, ct);
		return entity is null ? null : ToModel(entity);
	}

	public async Task<IReadOnlyCollection<TModel>> GetByIds(IReadOnlyCollection<string> ids, CancellationToken ct)
	{
		var entities = await GetEntitiesByIdsCore(ids, ct);
		return entities.Select(ToModel).ToList().AsReadOnly();
	}
}




internal abstract class SpotifyIdEntityService<TModel, TIdEntity, TPayload> : SpotifyIdEntityStoreService<TModel, TIdEntity>, ISpotifyIdEntityService<TModel, TPayload> where TModel : SpotifyIdNameObject
where TIdEntity : ISpotifyDb, ISpotifyIdEntity
where TPayload : ISpotifyPayload
{
	protected abstract TModel ToModel(TIdEntity entity, TPayload payload);

	public async Task<TModel?> GetById(TPayload payload, CancellationToken ct)
	{
		var entity = await GetEntityByIdCore(payload.Id, ct);
		return entity is null ? null : ToModel(entity, payload);
	}

	public async Task<IReadOnlyCollection<TModel>> GetByIds(IReadOnlyCollection<TPayload> payloads, CancellationToken ct)
	{
		if (payloads.Count == 0)
		{
			return [];
		}

		var ids = payloads.Select(p => p.Id).ToHashSet();

		await GetEntitiesByIdsCore(ids, ct);

		var result = new List<TModel>(payloads.Count);

		foreach (var payload in payloads)
		{
			if (TryGetCached(payload.Id, out var entity))
			{
				result.Add(ToModel(entity, payload));
			}
		}

		return result.AsReadOnly();
	}
}