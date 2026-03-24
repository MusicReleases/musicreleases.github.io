using DexieNET;
using JakubKastner.MusicReleases.Spotify;

namespace JakubKastner.MusicReleases.Database.Spotify.LinkEntities;

internal abstract class SpotifyLinkTwoKeysEntityService<TEntity, TPayload1, TPayload2> : SpotifyLinkOneKeyEntityService<TEntity, TPayload1>
	where TEntity : class, ISpotifyDb
	where TPayload1 : ISpotifyPayload
	where TPayload2 : ISpotifyPayload
{
	private readonly Dictionary<string, SortedSet<TPayload2>> _cacheByKey2 = [];

	protected abstract TEntity ToEntityFromKey2(TPayload2 payload, string key2);

	protected abstract TPayload2 ToPayload2(TEntity entity);

	protected SortedSet<TPayload2> GetOrCreateCacheByKey2(string key2)
	{
		if (!_cacheByKey2.TryGetValue(key2, out var set))
		{
			set = [];
			_cacheByKey2[key2] = set;
		}

		return set;
	}

	protected void InvalidateKey2(string key2)
	{
		_cacheByKey2.Remove(key2);
	}

	protected async Task<SortedSet<TPayload2>> GetByKey2(string key2, CancellationToken ct)
	{
		if (_cacheByKey2.TryGetValue(key2, out var cached))
		{
			return cached;
		}

		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		var entities = await table
			.Where(Key2Expression, key2)
			.ToArray();

		var payloads = new SortedSet<TPayload2>(entities.Select(ToPayload2));

		_cacheByKey2[key2] = payloads;

		return payloads;
	}

	protected async Task<IEnumerable<TEntity>> GetByKeys2(IEnumerable<string> keys2, CancellationToken ct)
	{
		return await GetByKeys(keys2, Key2Expression, ct);
	}

	public async Task SaveByKey2(string key2, IEnumerable<TPayload2> payloads, CancellationToken ct)
	{
		var incoming = payloads.ToList();

		var incomingIds = incoming.Select(p => p.Id).ToHashSet();

		var existing = await GetByKey2(key2, ct);

		var existingIds = existing.Select(p => p.Id).ToHashSet();

		var toAdd = incoming.Where(p => !existingIds.Contains(p.Id)).ToList();

		var toRemove = existingIds.Where(id => !incomingIds.Contains(id)).ToList();

		if (toAdd.Count == 0 && toRemove.Count == 0)
		{
			return;
		}

		await ApplyChangesByKey2(key2, toAdd, toRemove, ct);

		var cache = GetOrCreateCacheByKey2(key2);

		foreach (var payload in toAdd)
		{
			cache.Add(payload);
		}

		foreach (var id in toRemove)
		{
			cache.RemoveWhere(p => p.Id == id);
		}
	}

	public async Task SaveByKey2(TPayload2 payload, string key2, CancellationToken ct)
	{
		var entity = ToEntityFromKey2(payload, key2);

		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		await table.PutSafe(entity);

		var cache = GetOrCreateCacheByKey2(key2);

		cache.Add(payload);
	}

	private async Task ApplyChangesByKey2(string key2, List<TPayload2> toAdd, List<string> toRemove, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		if (toAdd.Count > 0)
		{
			var entitiesToAdd = toAdd.Select(p => ToEntityFromKey2(p, key2)).ToList();

			await table.BulkPutSafe(entitiesToAdd);
		}

		if (toRemove.Count > 0)
		{
			var keys = toRemove.Select(id => (key2, id)).ToArray();

			await table.Where(Key2Expression, Key1Expression).AnyOf(keys).Delete();
		}
	}

	public async Task DeleteAllByKey2(string key2)
	{
		var table = await GetTable();

		await table.Where(Key2Expression, key2).Delete();

		InvalidateKey2(key2);
	}

	public new async Task DeleteAll()
	{
		await base.DeleteAll();

		_cacheByKey2.Clear();
	}
}