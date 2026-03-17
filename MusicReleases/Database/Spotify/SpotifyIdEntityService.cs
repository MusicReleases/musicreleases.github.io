using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.MusicReleases.Spotify;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Database.Spotify;

internal abstract class SpotifyIdEntityService<TModel, TIdEntity, TPayload> : ISpotifyIdEntityService<TModel, TPayload> where TModel : SpotifyIdNameObject
	where TIdEntity : ISpotifyDb, ISpotifyIdEntity
	where TPayload : ISpotifyPayload
{
	protected readonly Dictionary<string, TModel> _cache = [];

	protected abstract Task<Table<TIdEntity, string>> GetTable();

	protected abstract TIdEntity ToEntity(TModel model);
	protected abstract TModel ToModel(TIdEntity entity, TPayload payload);

	protected abstract Task<IEnumerable<TIdEntity>> FetchByIds(string[] ids);

	public async Task<IReadOnlyCollection<TModel>> GetByIds(IReadOnlyCollection<TPayload> payloads, CancellationToken ct)
	{
		if (payloads.Count == 0)
		{
			return [];
		}

		var ids = payloads.Select(p => p.Id);
		var missingIds = ids.Where(id => !_cache.ContainsKey(id)).ToArray();

		if (missingIds.Length > 0)
		{
			var payloadMap = payloads.ToDictionary(p => p.Id);

			var table = await GetTable();
			ct.ThrowIfCancellationRequested();

			var itemsDb = await FetchByIds(missingIds);
			var items = itemsDb.Select(x => ToModel(x, payloadMap[x.Id]));

			AddToCache(items);
		}

		var filteredItems = ids.Where(_cache.ContainsKey).Select(id => _cache[id]).ToList();

		return filteredItems.AsReadOnly();
	}

	public async Task Save(IReadOnlyCollection<TModel> items, CancellationToken ct)
	{
		if (items.Count == 0)
		{
			return;
		}

		var itemsDb = items.Select(ToEntity);

		var table = await GetTable();
		ct.ThrowIfCancellationRequested();

		await table.BulkPutSafe(itemsDb);

		AddToCache(items);
	}

	public async Task Save(TModel item, CancellationToken ct)
	{
		var itemDb = ToEntity(item);

		var table = await GetTable();
		ct.ThrowIfCancellationRequested();

		await table.PutSafe(itemDb);

		AddToCache(item);
	}

	protected void AddToCache(IEnumerable<TModel> items)
	{
		foreach (var item in items)
		{
			AddToCache(item);
		}
	}

	protected void AddToCache(TModel item)
	{
		_cache[item.Id] = item;
	}

}