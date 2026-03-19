using DexieNET;

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