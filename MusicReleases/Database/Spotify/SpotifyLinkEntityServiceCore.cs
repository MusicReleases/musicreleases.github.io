using DexieNET;

namespace JakubKastner.MusicReleases.Database.Spotify;

internal abstract class SpotifyLinkEntityServiceCore<TEntity> : ISpotifyLinkEntityServiceCore where TEntity : ISpotifyDb
{

	protected abstract Task<Table<TEntity, (string, string)>> GetTable();

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