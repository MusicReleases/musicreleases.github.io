using DexieNET;
using JakubKastner.MusicReleases.Spotify;
using System.Linq.Expressions;

namespace JakubKastner.MusicReleases.Database.Spotify.LinkEntities;

internal abstract class SpotifyLinkOneKeyEntityService<TEntity, TPayload1> : ISpotifyLinkOneKeyEntityService
	where TEntity : ISpotifyDb
	where TPayload1 : ISpotifyPayload
{

	private readonly Dictionary<string, SortedSet<TPayload1>> _cacheByKey1 = [];

	protected abstract Expression<Func<TEntity, string>> Key1Expression { get; }

	protected abstract Expression<Func<TEntity, string>> Key2Expression { get; }

	protected abstract Task<Table<TEntity, (string, string)>> GetTable();

	protected abstract TEntity ToEntityFromKey1(TPayload1 payload, string key1);


	protected abstract TPayload1 ToPayload1(TEntity entity);

	protected async Task<IEnumerable<TEntity>> GetByKeys1(IEnumerable<string> keys1, CancellationToken ct)
	{
		return await GetByKeys(keys1, Key1Expression, ct);
	}

	protected async Task<IEnumerable<TEntity>> GetByKeys(IEnumerable<string> keys, Expression<Func<TEntity, string>> expression, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		var links = await table.Where(expression).AnyOf([.. keys]).ToArray();

		return links;
	}

	protected async Task<SortedSet<TPayload1>> GetByKey1(string key1, CancellationToken ct)
	{

		return await GetByKey(key1, Key1Expression, _cacheByKey1, ToPayload1, ct);
	}

	protected async Task<SortedSet<TPayload>> GetByKey<TPayload>(string key, Expression<Func<TEntity, string>> expression, Dictionary<string, SortedSet<TPayload>> cacheByKey, Func<TEntity, TPayload> toPayload, CancellationToken ct) where TPayload : ISpotifyPayload
	{
		if (cacheByKey.TryGetValue(key, out var cached))
		{
			return cached;
		}
		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		var links = await table.Where(expression, key).ToArray();

		var payloads = new SortedSet<TPayload>(links.Select(toPayload));
		cacheByKey[key] = payloads;

		return payloads;
	}

	public async Task SaveByKey1(string key1, IEnumerable<TPayload1> payloads, CancellationToken ct)
	{
		await SaveByKey(key1, payloads, Key1Expression, _cacheByKey1, ToEntityFromKey1, ToPayload1, Key1Expression, Key2Expression, ct);
	}

	protected async Task SaveByKey<TPayload>(string userId, IEnumerable<TPayload> payloads, Expression<Func<TEntity, string>> expression, Dictionary<string, SortedSet<TPayload>> cacheByKey, Func<TPayload, string, TEntity> toEntity, Func<TEntity, TPayload> toPayload, Expression<Func<TEntity, string>> keyExpression, Expression<Func<TEntity, string>> keySecondaryExpression, CancellationToken ct) where TPayload : ISpotifyPayload
	{
		var incoming = payloads.ToList();

		var incomingIds = incoming.Select(x => x.Id).ToHashSet();
		var currentIds = (await GetByKey(userId, expression, cacheByKey, toPayload, ct)).Select(x => x.Id).ToHashSet();

		var toAdd = incoming.Where(p => !currentIds.Contains(p.Id)).ToList();
		var toRemove = currentIds.Except(incomingIds).ToList();

		if (toAdd.Count == 0 && toRemove.Count == 0)
		{
			return;
		}

		await ApplyChangesByKey(userId, toAdd, toRemove, toEntity, keyExpression, keySecondaryExpression, ct);

		if (cacheByKey.TryGetValue(userId, out var cachedSet))
		{
			cachedSet.UnionWith(toAdd);
			cachedSet.RemoveWhere(x => toRemove.Contains(x.Id));
		}
	}

	private async Task ApplyChangesByKey1(string key1, List<TPayload1> toAdd, List<string> toRemove, CancellationToken ct)
	{
		await ApplyChangesByKey(key1, toAdd, toRemove, ToEntityFromKey1, Key1Expression, Key2Expression, ct);
	}

	protected async Task ApplyChangesByKey<TPayload>(string key, List<TPayload> toAdd, List<string> toRemove, Func<TPayload, string, TEntity> toEntity, Expression<Func<TEntity, string>> keyMainExpression, Expression<Func<TEntity, string>> keySecondaryExpression, CancellationToken ct) where TPayload : ISpotifyPayload
	{
		ct.ThrowIfCancellationRequested();
		var table = await GetTable();

		if (toAdd.Count > 0)
		{
			var toAddDb = toAdd.Select(payload => toEntity(payload, key));

			await table.BulkPutSafe(toAddDb);
		}

		if (toRemove.Count > 0)
		{
			var keys = toRemove.Select(linkId => (key, linkId)).ToArray();

			await table.Where(keyMainExpression, keySecondaryExpression).AnyOf(keys).Delete();
		}
	}

	public async Task SaveByKey1(IReadOnlyCollection<TPayload1> items, string key1, CancellationToken ct)
	{
		await SaveByKey(items, key1, ToEntityFromKey1, _cacheByKey1, ct);
	}


	protected async Task SaveByKey<TPayload>(IReadOnlyCollection<TPayload> items, string key, Func<TPayload, string, TEntity> toEntity, Dictionary<string, SortedSet<TPayload>> cache, CancellationToken ct) where TPayload : ISpotifyPayload
	{
		if (items.Count == 0)
		{
			return;
		}

		var itemsDb = items.Select(x => toEntity(x, key)).ToList();

		await SaveEntities(itemsDb, ct);

		AddToCache(items, key, cache);
	}

	public async Task SaveByKey1(TPayload1 item, string key1, CancellationToken ct)
	{
		var itemDb = ToEntityFromKey1(item, key1);

		await SaveEntity(itemDb, ct);

		AddToCache(item, key1, _cacheByKey1);
	}



	protected void AddToCache<TPayload>(IEnumerable<TPayload> items, string key, Dictionary<string, SortedSet<TPayload>> cacheByKey) where TPayload : ISpotifyPayload
	{
		cacheByKey[key].UnionWith(items);
	}

	protected void AddToCache<TPayload>(TPayload item, string key, Dictionary<string, SortedSet<TPayload>> cacheByKey) where TPayload : ISpotifyPayload
	{
		cacheByKey[key].Add(item);
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

	public async Task DeleteAllByKey1(string key1)
	{
		var table = await GetTable();
		await table.Where(Key1Expression, key1).Delete();

		_cacheByKey1.Remove(key1);
	}
}