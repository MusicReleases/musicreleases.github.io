using DexieNET;
using JakubKastner.MusicReleases.Spotify;

namespace JakubKastner.MusicReleases.Database.Spotify;

internal abstract class SpotifyLinkEntityService<TEntity, TKey1, TKey2> : ISpotifyLinkEntityService where TEntity : ISpotifyDb
	where TKey1 : notnull
	where TKey2 : notnull
{
	protected abstract Task<Table<TEntity, (TKey1, TKey2)>> GetTable();
	protected abstract Task<IEnumerable<TEntity>> FetchByKeys1(IEnumerable<TKey1> keys1);

	protected async Task SaveEntities(IReadOnlyCollection<TEntity> entities, CancellationToken ct)
	{
		if (entities.Count == 0)
		{
			return;
		}

		var table = await GetTable();
		ct.ThrowIfCancellationRequested();

		await table.BulkPutSafe(entities);
	}

	protected async Task SaveEntity(TEntity entity, CancellationToken ct)
	{
		var table = await GetTable();
		ct.ThrowIfCancellationRequested();

		await table.PutSafe(entity);
	}

	public async Task DeleteAll()
	{
		var table = await GetTable();
		await table.Clear();
	}
}


internal abstract class SpotifyLinkEntityService2<TEntity, TPayload>
	where TEntity : ISpotifyDb
	where TPayload : ISpotifyLinkPayload
{
	private readonly Dictionary<string, List<TPayload>> _cache = [];

	protected virtual bool InsertOnly => true;

	protected abstract TPayload ToPayload(TEntity entity);
	protected abstract TEntity ToEntity(TPayload payload);

	protected abstract Task<Table<TEntity, (string, string)>> GetTable();
	protected abstract Task<IEnumerable<TEntity>> FetchByKey1s(IEnumerable<string> keys1);

	public async Task<IReadOnlyCollection<TPayload>> GetByKeys1(IEnumerable<string> keys1, CancellationToken ct)
	{
		var key1List = keys1.ToList();
		var missing = key1List.Where(k => !_cache.ContainsKey(k)).ToList();

		if (missing.Count > 0)
		{
			ct.ThrowIfCancellationRequested();
			var linksDb = await FetchByKey1s(missing);

			var payloads = linksDb.Select(ToPayload).ToList();

			foreach (var group in payloads.GroupBy(x => x.Id))
			{
				_cache[group.Key] = [.. group];
			}
		}

		return key1List.Where(_cache.ContainsKey).SelectMany(k => _cache[k]).ToList().AsReadOnly();
	}

	public async Task Save(IReadOnlyCollection<TPayload> payloads, CancellationToken ct)
	{
		if (payloads.Count == 0)
		{
			return;
		}

		var table = await GetTable();
		List<TPayload> toSave;

		if (InsertOnly)
		{
			// cache first
			var notInCache = payloads.Where(p =>
				!_cache.TryGetValue(p.Id, out var cached) ||
				cached.All(c => c.Key2?.Equals(p.Key2) == false)
			).ToList();

			if (notInCache.Count == 0)
			{
				return;
			}

			var keys = notInCache.Select(p => (p.Id, p.Key2)).ToArray();
			var existingKeys = (await table.BulkGet(keys))
				.Where(e => e is not null)
				.Select(e => ToPayload(e!))
				.Select(p => (p.Id, p.Key2))
				.ToHashSet();

			toSave = [.. notInCache.Where(p => !existingKeys.Contains((p.Id, p.Key2)))];

			if (toSave.Count == 0)
			{
				return;
			}
		}
		else
		{
			toSave = [.. payloads];
		}

		ct.ThrowIfCancellationRequested();

		await table.BulkPutSafe(toSave.Select(ToEntity));

		AddToCache(toSave);
	}

	private void AddToCache(IEnumerable<TPayload> payloads)
	{
		foreach (var group in payloads.GroupBy(x => x.Id))
		{
			if (_cache.TryGetValue(group.Key, out var existing))
			{
				existing.AddRange(group);
			}
			else
			{
				_cache[group.Key] = [.. group];
			}
		}
	}
}
