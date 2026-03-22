using DexieNET;
using JakubKastner.MusicReleases.Spotify;

namespace JakubKastner.MusicReleases.Database.Spotify.LinkEntities;

internal abstract class SpotifyLinkTwoKeysEntityService<TEntity, TPayload1, TPayload2> : SpotifyLinkOneKeyEntityService<TEntity, TPayload1>, ISpotifyLinkTwoKeysEntityService<TPayload2> where TEntity : ISpotifyDb
	where TPayload1 : ISpotifyPayload
	where TPayload2 : ISpotifyPayload
{
	private readonly Dictionary<string, SortedSet<TPayload2>> _cacheByKey2 = [];

	protected abstract TEntity ToEntityFromKey2(TPayload2 payload, string key2);

	protected abstract TPayload2 ToPayload2(TEntity entity);

	protected async Task<IEnumerable<TEntity>> GetByKeys2(IEnumerable<string> keys2, CancellationToken ct)
	{
		return await GetByKeys(keys2, Key2Expression, ct);
	}

	protected async Task<SortedSet<TPayload2>> GetByKey2(string key2, CancellationToken ct)
	{
		return await GetByKey(key2, Key2Expression, _cacheByKey2, ToPayload2, ct);
	}

	public async Task SaveByKey2(string key2, IEnumerable<TPayload2> payloads, CancellationToken ct)
	{
		await SaveByKey(key2, payloads, Key2Expression, _cacheByKey2, ToEntityFromKey2, ToPayload2, Key2Expression, Key1Expression, ct);
	}

	public async Task SaveByKey2(IReadOnlyCollection<TPayload2> items, string key2, CancellationToken ct)
	{
		await SaveByKey(items, key2, ToEntityFromKey2, _cacheByKey2, ct);
	}

	private async Task ApplyChangesByKey2(string key2, List<TPayload2> toAdd, List<string> toRemove, CancellationToken ct)
	{
		await ApplyChangesByKey(key2, toAdd, toRemove, ToEntityFromKey2, Key2Expression, Key1Expression, ct);
	}

	public async Task SaveByKey2(TPayload2 item, string key2, CancellationToken ct)
	{
		var itemDb = ToEntityFromKey2(item, key2);

		await SaveEntity(itemDb, ct);

		AddToCache(item, key2, _cacheByKey2);
	}

	public async Task DeleteAllByKey2(string key2)
	{
		var table = await GetTable();
		await table.Where(Key2Expression, key2).Delete();

		_cacheByKey2.Remove(key2);
	}
}