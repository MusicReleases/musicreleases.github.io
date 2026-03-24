namespace JakubKastner.MusicReleases.Database.Spotify.BaseServices;

internal abstract class KeyedEntityServiceCore<TEntity, TKey>
	where TEntity : class, ISpotifyDb
	where TKey : notnull
{
	private readonly Dictionary<TKey, TEntity> _cache = [];

	protected abstract TKey GetKey(TEntity entity);

	protected abstract Task<TEntity?> FetchByKey(TKey key, CancellationToken ct);

	protected abstract Task<IReadOnlyCollection<TEntity>> FetchByKeys(IReadOnlyCollection<TKey> keys, CancellationToken ct);

	protected bool TryGetCached(TKey key, out TEntity entity)
	{
		return _cache.TryGetValue(key, out entity!);
	}

	protected bool ContainsCached(TKey key)
	{
		return _cache.ContainsKey(key);
	}

	protected void SetCache(TEntity entity)
	{
		var key = GetKey(entity);
		_cache[key] = entity;
	}

	protected void SetCache(IEnumerable<TEntity> entities)
	{
		foreach (var entity in entities)
		{
			var key = GetKey(entity);
			_cache[key] = entity;
		}
	}

	protected void Invalidate(TKey key)
	{
		_cache.Remove(key);
	}

	protected void ClearCache()
	{
		_cache.Clear();
	}

	protected async Task<TEntity?> GetEntityCore(TKey key, bool useCache, CancellationToken ct)
	{
		if (useCache)
		{
			if (TryGetCached(key, out var cached))
			{
				return cached;
			}
		}

		ct.ThrowIfCancellationRequested();

		var entity = await FetchByKey(key, ct);

		if (entity is not null)
		{
			SetCache(entity);
		}

		return entity;
	}

	protected async Task<IReadOnlyCollection<TEntity>> GetEntitiesCore(IReadOnlyCollection<TKey> keys, bool useCache, CancellationToken ct)
	{
		if (keys.Count == 0)
		{
			return [];
		}

		var distinctKeys = keys.ToHashSet();

		List<TKey> missingKeys;

		if (useCache)
		{
			missingKeys = distinctKeys.Where(k => !_cache.ContainsKey(k)).ToList();
		}
		else
		{
			missingKeys = distinctKeys.ToList();
		}

		if (missingKeys.Count > 0)
		{
			ct.ThrowIfCancellationRequested();

			var fetched = await FetchByKeys(missingKeys, ct);

			SetCache(fetched);
		}

		var result = new List<TEntity>(distinctKeys.Count);

		foreach (var key in distinctKeys)
		{
			if (_cache.TryGetValue(key, out var entity))
			{
				result.Add(entity);
			}
		}

		return result.AsReadOnly();
	}
}