using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.MusicReleases.Spotify;

namespace JakubKastner.MusicReleases.Database.Spotify;

internal abstract class SpotifyUserLinkEntityService<TUserIdEntity, TPayload> : SpotifyLinkEntityService<TUserIdEntity, string, string>, ISpotifyUserLinkEntityService<TPayload>
	where TUserIdEntity : ISpotifyDb, ISpotifyUserIdEntity
	where TPayload : ISpotifyPayload
{
	private readonly Dictionary<string, SortedSet<TPayload>> _cache = [];

	protected abstract TPayload ToPayload(TUserIdEntity entity);

	protected abstract TUserIdEntity ToEntity(TPayload payload, string userId);

	protected abstract Task<IEnumerable<TUserIdEntity>> FetchByUserId(string userId);

	protected sealed override async Task<IEnumerable<TUserIdEntity>> FetchByKeys1(IEnumerable<string> userIds)
	{
		var results = new List<TUserIdEntity>();
		foreach (var userId in userIds)
		{
			results.AddRange(await FetchByUserId(userId));
		}
		return results;
	}


	public async Task<IReadOnlyCollection<TPayload>> GetByUserId(string userId, CancellationToken ct)
	{
		if (_cache.TryGetValue(userId, out var cached))
		{
			return cached;
		}

		ct.ThrowIfCancellationRequested();
		var linksDb = await FetchByUserId(userId);

		var payloads = new SortedSet<TPayload>(linksDb.Select(ToPayload));
		_cache[userId] = payloads;

		return payloads;
	}

	public async Task SaveByUserId(string userId, IEnumerable<TPayload> payloads, CancellationToken ct)
	{
		var incoming = payloads.ToList();

		var incomingIds = incoming.Select(x => x.Id).ToHashSet();
		var currentIds = (await GetByUserId(userId, ct)).Select(x => x.Id).ToHashSet();

		var toAdd = incoming.Where(p => !currentIds.Contains(p.Id)).ToList();
		var toRemove = currentIds.Except(incomingIds).ToList();

		if (toAdd.Count == 0 && toRemove.Count == 0)
		{
			return;
		}

		await ApplyChanges(userId, toAdd, toRemove, ct);

		if (_cache.TryGetValue(userId, out var cachedSet))
		{
			cachedSet.UnionWith(toAdd);
			cachedSet.RemoveWhere(x => toRemove.Contains(x.Id));
		}
	}

	private async Task ApplyChanges(string userId, List<TPayload> toAdd, List<string> toRemove, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();
		var table = await GetTable();

		if (toAdd.Count > 0)
		{
			var toAddDb = toAdd.Select(payload => ToEntity(payload, userId));

			await table.BulkPutSafe(toAddDb);
		}

		if (toRemove.Count > 0)
		{
			var keys = toRemove.Select(linkId => (userId, linkId)).ToArray();

			await table.BulkDelete(keys);
		}
	}

	public async Task Save(IReadOnlyCollection<TPayload> items, string userId, CancellationToken ct)
	{
		if (items.Count == 0)
		{
			return;
		}

		var itemsDb = items.Select(x => ToEntity(x, userId)).ToList();

		await SaveEntities(itemsDb, ct);

		AddToCache(items, userId);
	}

	public async Task Save(TPayload item, string userId, CancellationToken ct)
	{
		var itemDb = ToEntity(item, userId);

		await SaveEntity(itemDb, ct);

		AddToCache(item, userId);
	}

	protected void AddToCache(IEnumerable<TPayload> items, string userId)
	{
		_cache[userId].UnionWith(items);
	}

	protected void AddToCache(TPayload item, string userId)
	{
		_cache[userId].Add(item);
	}

	public async Task DeleteAllForUser(string userId)
	{
		var table = await GetTable();
		await table.Where(x => x.UserId, userId).Delete();

		_cache.Remove(userId);
	}
}