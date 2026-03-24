using DexieNET;
using System.Linq.Expressions;

namespace JakubKastner.MusicReleases.Database.Spotify.BaseServices;

internal abstract class IdEntityStoreBase<TEntity> : SpotifyKeyedEntityServiceCore<TEntity, string>
	where TEntity : class, ISpotifyDb
{
	protected abstract Task<Table<TEntity, string>> GetTable();

	protected abstract Expression<Func<TEntity, string>> IdExpression { get; }

	protected abstract string GetEntityId(TEntity entity);

	protected override string GetKey(TEntity entity)
	{
		return GetEntityId(entity);
	}

	protected override async Task<TEntity?> FetchByKey(string id, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		var entity = await table.Get(id);

		return entity;
	}

	protected override async Task<IReadOnlyCollection<TEntity>> FetchByKeys(IReadOnlyCollection<string> ids, CancellationToken ct)
	{
		if (ids.Count == 0)
		{
			return [];
		}

		var table = await GetTable();

		ct.ThrowIfCancellationRequested();

		var entities = await table.BulkGet(ids);

		return entities.Where(e => e is not null).ToList().AsReadOnly();
	}

	protected async Task SaveEntity(TEntity entity, CancellationToken ct)
	{
		var table = await GetTable();

		ct.ThrowIfCancellationRequested();

		await table.PutSafe(entity);

		SetCache(entity);
	}

	protected async Task SaveEntities(IReadOnlyCollection<TEntity> entities, CancellationToken ct)
	{
		if (entities.Count == 0)
		{
			return;
		}

		var table = await GetTable();

		ct.ThrowIfCancellationRequested();

		await table.BulkPutSafe(entities);

		SetCache(entities);
	}

	protected async Task DeleteEntity(string id, CancellationToken ct)
	{
		var table = await GetTable();

		ct.ThrowIfCancellationRequested();

		await table.Delete(id);

		Invalidate(id);
	}

	protected async Task UpdateEntity<TValue>(string id, Expression<Func<TEntity, TValue>> selector, TValue newValue, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		await table.Update(id, selector, newValue);

		var updated = await FetchByKey(id, ct);

		if (updated is not null)
		{
			SetCache(updated);
		}
		else
		{
			Invalidate(id);
		}
	}
}