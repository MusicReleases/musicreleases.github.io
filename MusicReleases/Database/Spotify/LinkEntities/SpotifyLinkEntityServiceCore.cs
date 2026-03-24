using DexieNET;

namespace JakubKastner.MusicReleases.Database.Spotify.LinkEntities;

internal abstract class SpotifyLinkEntityServiceCore<TEntity>
	where TEntity : class, ISpotifyDb
{
	protected abstract Task<Table<TEntity, (string, string)>> GetTable();

	protected async Task SaveEntities(IReadOnlyCollection<TEntity> entities, CancellationToken ct)
	{
		if (entities.Count == 0)
		{
			return;
		}

		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		await table.BulkPutSafe(entities);
	}

	protected async Task SaveEntity(TEntity entity, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		await table.PutSafe(entity);
	}

	public async Task DeleteAll(CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		await table.Clear();
	}
}