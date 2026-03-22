using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Database.Spotify.IdEntities;

internal abstract class SpotifyIdEntityStoreService<TModel, TIdEntity> : SpotifyIdEntityServiceCore<TIdEntity>, ISpotifyIdEntityStoreService<TModel> where TModel : SpotifyIdNameObject
	where TIdEntity : ISpotifyDb, ISpotifyIdEntity
{
	protected abstract TIdEntity ToEntity(TModel model);

	protected abstract Task<Table<TIdEntity, string>> GetTable();


	protected override async Task<TIdEntity?> FetchById(string id, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();
		var table = await GetTable();

		var itemDb = await table.Get(id);

		return itemDb;
	}

	protected override async Task<IReadOnlyCollection<TIdEntity>> FetchByIds(IReadOnlyCollection<string> ids, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();
		var table = await GetTable();

		var itemsDb = await table.BulkGet(ids);

		return itemsDb.ToList().AsReadOnly();
	}

	public async Task Save(TModel item, bool keepExisting, CancellationToken ct)
	{
		if (keepExisting && ContainsCached(item.Id))
		{
			// keep existing item with same id
			return;
		}

		var itemDb = ToEntity(item);

		ct.ThrowIfCancellationRequested();
		var table = await GetTable();

		await table.PutSafe(itemDb);

		AddToCache(itemDb);
	}

	public async Task Save(IReadOnlyCollection<TModel> items, bool keepExisting, CancellationToken ct)
	{
		if (items.Count == 0)
		{
			return;
		}

		List<TModel>? itemsToAdd;
		var table = await GetTable();

		if (keepExisting)
		{
			// keep existing items with same ids
			var notInCache = items.Where(x => !ContainsCached(x.Id)).ToList();

			if (notInCache.Count == 0)
			{
				return;
			}

			var notInCacheIds = notInCache.Select(x => x.Id).ToArray();

			ct.ThrowIfCancellationRequested();

			var existingIds = (await table.Where(x => x.Id).AnyOf(notInCacheIds).Keys()).ToHashSet();

			itemsToAdd = [.. items.Where(x => !existingIds.Contains(x.Id))];

			if (itemsToAdd.Count == 0)
			{
				return;
			}
		}
		else
		{
			//  revrite all existing items
			itemsToAdd = [.. items];
		}

		var itemsDb = itemsToAdd.Select(ToEntity);

		ct.ThrowIfCancellationRequested();

		await table.BulkPutSafe(itemsDb);

		AddToCache(itemsDb);
	}
}