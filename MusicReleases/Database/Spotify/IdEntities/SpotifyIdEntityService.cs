namespace JakubKastner.MusicReleases.Database.Spotify.IdEntities;
/*
internal abstract class SpotifyIdEntityService<TModel, TEntity> : KeyedEntityService<TModel, TEntity>
	where TModel : class
	where TEntity : class, ISpotifyDb
{
	protected abstract TModel ToModel(TEntity entity);

	public async Task<TModel?> GetById(string id, CancellationToken ct)
	{
		var entity = await GetEntityByIdCore(id, ct);

		if (entity is null)
		{
			return null;
		}

		return ToModel(entity);
	}

	public async Task<IReadOnlyCollection<TModel>> GetByIds(IReadOnlyCollection<string> ids, CancellationToken ct)
	{
		if (ids.Count == 0)
		{
			return [];
		}

		var entities = await GetEntitiesByIdsCore(ids, ct);

		var result = new List<TModel>(entities.Count);

		foreach (var entity in entities)
		{
			result.Add(ToModel(entity));
		}

		return result.AsReadOnly();
	}
}

internal abstract class SpotifyIdEntityService<TModel, TEntity, TPayload> : KeyedEntityService<TModel, TEntity>
	where TModel : SpotifyIdObject
	where TEntity : class, ISpotifyDb
	where TPayload : ISpotifyPayload
{
	protected abstract TModel ToModel(TEntity entity, TPayload payload);

	public async Task<TModel?> GetById(TPayload payload, CancellationToken ct)
	{
		var entity = await GetEntityByIdCore(payload.Id, ct);

		if (entity is null)
		{
			return null;
		}

		return ToModel(entity, payload);
	}

	public async Task<IReadOnlyCollection<TModel>> GetByIds(IReadOnlyCollection<TPayload> payloads, CancellationToken ct)
	{
		if (payloads.Count == 0)
		{
			return [];
		}

		var ids = payloads
			.Select(p => p.Id)
			.ToHashSet();

		await GetEntitiesByIdsCore(ids, ct);

		var result = new List<TModel>(payloads.Count);

		foreach (var payload in payloads)
		{
			if (TryGetCached(payload.Id, out var entity))
			{
				if (entity is not null)
				{
					result.Add(ToModel(entity, payload));
				}
			}
		}

		return result.AsReadOnly();
	}
}*/