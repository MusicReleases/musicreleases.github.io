using DexieNET;
using JakubKastner.MusicReleases.Spotify;
using System.Linq.Expressions;

namespace JakubKastner.MusicReleases.Database.Spotify.LinkEntities;

internal abstract class SpotifyLinkOneKeyEntityService<TEntity, TPayload>
	where TEntity : class, ISpotifyDb
	where TPayload : ISpotifyPayload
{
	private readonly Dictionary<string, SortedSet<TPayload>> _cacheByKey1 = [];

	protected abstract Expression<Func<TEntity, string>> Key1Expression { get; }

	protected abstract Expression<Func<TEntity, string>> Key2Expression { get; }

	protected abstract Task<Table<TEntity, (string, string)>> GetTable();

	protected abstract TEntity ToEntityFromKey1(TPayload payload, string key1);

	protected abstract TPayload ToPayload1(TEntity entity);

	protected SortedSet<TPayload> GetOrCreateCacheByKey1(string key1)
	{
		if (!_cacheByKey1.TryGetValue(key1, out var set))
		{
			set = [];
			_cacheByKey1[key1] = set;
		}

		return set;
	}

	protected void InvalidateKey1(string key1)
	{
		_cacheByKey1.Remove(key1);
	}

	protected async Task<SortedSet<TPayload>> GetByKey1(string key1, CancellationToken ct)
	{
		if (_cacheByKey1.TryGetValue(key1, out var cached))
		{
			return cached;
		}

		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		var entities = await table
			.Where(Key1Expression, key1)
			.ToArray();

		var payloads = new SortedSet<TPayload>(entities.Select(ToPayload1));

		_cacheByKey1[key1] = payloads;

		return payloads;
	}

	protected async Task<IEnumerable<TEntity>> GetByKeys(IEnumerable<string> keys, Expression<Func<TEntity, string>> expression, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		var entities = await table.Where(expression).AnyOf(keys.ToArray()).ToArray();

		return entities;
	}

	public async Task SaveByKey1(string key1, IEnumerable<TPayload> payloads, CancellationToken ct)
	{
		var incoming = payloads.ToList();

		var incomingIds = incoming.Select(p => p.Id).ToHashSet();

		var existing = await GetByKey1(key1, ct);

		var existingIds = existing.Select(p => p.Id).ToHashSet();

		var toAdd = incoming.Where(p => !existingIds.Contains(p.Id)).ToList();

		var toRemove = existingIds.Where(id => !incomingIds.Contains(id)).ToList();

		if (toAdd.Count == 0 && toRemove.Count == 0)
		{
			return;
		}

		await ApplyChangesByKey1(key1, toAdd, toRemove, ct);

		var cache = GetOrCreateCacheByKey1(key1);

		foreach (var payload in toAdd)
		{
			cache.Add(payload);
		}

		foreach (var id in toRemove)
		{
			cache.RemoveWhere(p => p.Id == id);
		}
	}

	public async Task SaveByKey1(string key1, TPayload payload, CancellationToken ct)
	{
		var entity = ToEntityFromKey1(payload, key1);

		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		await table.PutSafe(entity);

		var cache = GetOrCreateCacheByKey1(key1);

		cache.Add(payload);
	}

	private async Task ApplyChangesByKey1(string key1, List<TPayload> toAdd, List<string> toRemove, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		if (toAdd.Count > 0)
		{
			var entitiesToAdd = toAdd.Select(p => ToEntityFromKey1(p, key1)).ToList();

			await table.BulkPutSafe(entitiesToAdd);
		}

		if (toRemove.Count > 0)
		{
			var keys = toRemove.Select(id => (key1, id)).ToArray();

			await table.Where(Key1Expression, Key2Expression).AnyOf(keys).Delete();
		}
	}

	public async Task DeleteAllByKey1(string key1)
	{
		var table = await GetTable();

		await table.Where(Key1Expression, key1).Delete();

		InvalidateKey1(key1);
	}

	public async Task DeleteAll()
	{
		var table = await GetTable();

		await table.Clear();

		_cacheByKey1.Clear();
	}
}