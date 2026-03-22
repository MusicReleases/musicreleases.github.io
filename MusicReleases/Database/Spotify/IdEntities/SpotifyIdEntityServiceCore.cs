using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using System.Diagnostics.CodeAnalysis;

namespace JakubKastner.MusicReleases.Database.Spotify.IdEntities;

internal abstract class SpotifyIdEntityServiceCore<TIdEntity>
	where TIdEntity : ISpotifyDb, ISpotifyIdEntity
{
	private readonly Dictionary<string, TIdEntity> _cache = [];

	protected abstract Task<TIdEntity?> FetchById(string id, CancellationToken ct);
	protected abstract Task<IReadOnlyCollection<TIdEntity>> FetchByIds(IReadOnlyCollection<string> ids, CancellationToken ct);

	protected bool TryGetCached(string id, [NotNullWhen(true)] out TIdEntity? entity) => _cache.TryGetValue(id, out entity);
	protected bool ContainsCached(string id) => _cache.ContainsKey(id);


	protected void AddToCache(IEnumerable<TIdEntity> items)
	{
		foreach (var item in items)
		{
			AddToCache(item);
		}
	}

	protected void AddToCache(TIdEntity item)
	{
		_cache[item.Id] = item;
	}


	protected async Task<TIdEntity?> GetEntityByIdCore(string id, CancellationToken ct)
	{
		if (TryGetCached(id, out var cached))
		{
			return cached;
		}

		var entity = await FetchById(id, ct);

		if (entity is not null)
		{
			AddToCache(entity);
		}

		return entity;
	}

	protected async Task<IReadOnlyCollection<TIdEntity>> GetEntitiesByIdsCore(IReadOnlyCollection<string> ids, CancellationToken ct)
	{
		if (ids.Count == 0)
		{
			return [];
		}

		var missingIds = ids.ToHashSet().Where(id => !_cache.ContainsKey(id)).ToArray();

		if (missingIds.Length > 0)
		{
			var itemsDb = await FetchByIds(missingIds, ct);

			AddToCache(itemsDb);
		}

		var result = new List<TIdEntity>(ids.Count);
		foreach (var id in ids)
		{
			if (TryGetCached(id, out var entity))
			{
				result.Add(entity);
			}
		}

		return result;
	}
}
