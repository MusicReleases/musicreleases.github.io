using DexieNET;
using JakubKastner.MusicReleases.Spotify;
using JakubKastner.SpotifyApi.Objects.Base;
using System.Linq.Expressions;

namespace JakubKastner.MusicReleases.Database.Spotify.BaseServices;

internal abstract class IdEntityStoreService<TModel, TEntity> : IdEntityStoreBase<TEntity>
	where TEntity : class, ISpotifyDb
	where TModel : class
{
	protected abstract string GetModelId(TModel model);

	protected abstract TEntity ToEntity(TModel model);

	protected abstract TModel ToModel(TEntity entity);

	public async Task<TModel?> GetById(string id, CancellationToken ct)
	{
		var entity = await GetEntityCore(id, true, ct);

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

		var entities = await GetEntitiesCore(ids, true, ct);

		var models = new List<TModel>(entities.Count);

		foreach (var entity in entities)
		{
			models.Add(ToModel(entity));
		}

		return models.AsReadOnly();
	}

	public async Task Save(TModel model, bool keepExisting, CancellationToken ct)
	{
		var id = GetModelId(model);

		if (keepExisting)
		{
			var existing = await GetEntityCore(id, true, ct);

			if (existing is not null)
			{
				return;
			}
		}

		var entity = ToEntity(model);

		await SaveEntity(entity, ct);
	}

	public async Task Save(IReadOnlyCollection<TModel> models, bool keepExisting, CancellationToken ct)
	{
		if (models.Count == 0)
		{
			return;
		}

		List<TEntity> entitiesToSave;

		if (keepExisting)
		{
			var ids = models
				.Select(GetModelId)
				.ToHashSet();

			var existingEntities = await GetEntitiesCore(ids, true, ct);

			var existingIds = existingEntities
				.Select(GetEntityId)
				.ToHashSet();

			entitiesToSave = models
				.Where(m => !existingIds.Contains(GetModelId(m)))
				.Select(ToEntity)
				.ToList();

			if (entitiesToSave.Count == 0)
			{
				return;
			}
		}
		else
		{
			entitiesToSave = models
				.Select(ToEntity)
				.ToList();
		}

		await SaveEntities(entitiesToSave, ct);
	}

	public async Task Delete(string id, CancellationToken ct)
	{
		await DeleteEntity(id, ct);
	}

	public async Task Update<TValue>(
		string id,
		Expression<Func<TEntity, TValue>> selector,
		TValue newValue,
		CancellationToken ct)
	{
		await UpdateEntity(id, selector, newValue, ct);
	}
}



internal abstract class IdEntityPayloadStoreService<TModel, TEntity, TPayload> : IdEntityStoreBase<TEntity>
	where TModel : SpotifyIdObject
	where TEntity : class, ISpotifyDb
	where TPayload : ISpotifyPayload
{
	protected abstract string GetModelId(TModel model);

	protected abstract TEntity ToEntity(TModel model);

	protected abstract TModel ToModel(TEntity entity, TPayload payload);

	public async Task<TModel?> GetById(TPayload payload, CancellationToken ct)
	{
		var entity = await GetEntityCore(payload.Id, true, ct);

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

		await GetEntitiesCore(ids, true, ct);

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

	public async Task Save(TModel model, bool keepExisting, CancellationToken ct)
	{
		var id = GetModelId(model);

		if (keepExisting)
		{
			var existing = await GetEntityCore(id, true, ct);

			if (existing is not null)
			{
				return;
			}
		}

		var entity = ToEntity(model);

		await SaveEntity(entity, ct);
	}

	public async Task Save(IReadOnlyCollection<TModel> models, bool keepExisting, CancellationToken ct)
	{
		if (models.Count == 0)
		{
			return;
		}

		List<TEntity> entitiesToSave;

		if (keepExisting)
		{
			var ids = models
				.Select(GetModelId)
				.ToHashSet();

			var existingEntities = await GetEntitiesCore(ids, true, ct);

			var existingIds = existingEntities
				.Select(GetEntityId)
				.ToHashSet();

			entitiesToSave = models
				.Where(m => !existingIds.Contains(GetModelId(m)))
				.Select(ToEntity)
				.ToList();

			if (entitiesToSave.Count == 0)
			{
				return;
			}
		}
		else
		{
			entitiesToSave = models
				.Select(ToEntity)
				.ToList();
		}

		await SaveEntities(entitiesToSave, ct);
	}

	public async Task Delete(string id, CancellationToken ct)
	{
		await DeleteEntity(id, ct);
	}

	public async Task Update<TValue>(
		string id,
		Expression<Func<TEntity, TValue>> selector,
		TValue newValue,
		CancellationToken ct)
	{
		await UpdateEntity(id, selector, newValue, ct);
	}
}