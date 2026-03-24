using DexieNET;
using JakubKastner.SpotifyApi.Clients;
using System.Linq.Expressions;

namespace JakubKastner.MusicReleases.Database.Spotify.BaseServices;

internal abstract class SpotifyUserScopedEntityService<TModel, TEntity>(ISpotifyUserClient userClient) : SpotifyKeyedEntityServiceCore<TEntity, string>, ISpotifyUserScopedEntityService<TModel>
	where TEntity : class, ISpotifyDb
	where TModel : class
{
	private readonly ISpotifyUserClient _userClient = userClient;

	protected string UserId => _userClient.GetUserIdRequired();

	protected abstract Task<Table<TEntity, string>> GetTable();

	protected abstract Expression<Func<TEntity, string>> UserIdExpression { get; }

	protected abstract string GetEntityUserId(TEntity entity);

	protected abstract TEntity ToEntity(TModel model, string userId);

	protected abstract TModel ToModel(TEntity entity);

	protected override string GetKey(TEntity entity)
	{
		return GetEntityUserId(entity);
	}

	protected override async Task<TEntity?> FetchByKey(string userId, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		var table = await GetTable();

		var entity = await table.Get(userId);

		return entity;
	}

	protected override async Task<IReadOnlyCollection<TEntity>> FetchByKeys(IReadOnlyCollection<string> userIds, CancellationToken ct)
	{
		ct.ThrowIfCancellationRequested();

		if (userIds.Count == 0)
		{
			return [];
		}

		var table = await GetTable();

		var entities = await table.BulkGet(userIds);

		return entities.Where(e => e is not null).ToList().AsReadOnly();
	}

	public Task<TModel?> Get(CancellationToken ct)
	{
		return GetByUserId(UserId, ct);
	}

	public async Task<TModel?> GetByUserId(string userId, CancellationToken ct)
	{
		var entity = await GetEntityCore(userId, true, ct);

		if (entity is null)
		{
			return null;
		}

		return ToModel(entity);
	}

	public async Task Save(TModel model, bool keepExisting, CancellationToken ct)
	{
		var userId = UserId;

		if (keepExisting)
		{
			var existing = await GetEntityCore(userId, true, ct);

			if (existing is not null)
			{
				return;
			}
		}

		var entity = ToEntity(model, userId);

		var table = await GetTable();

		ct.ThrowIfCancellationRequested();

		await table.PutSafe(entity);

		SetCache(entity);
	}

	public async Task DeleteForCurrentUser(CancellationToken ct)
	{
		await DeleteByUserId(UserId, ct);
	}

	public async Task DeleteByUserId(string userId, CancellationToken ct)
	{
		var table = await GetTable();

		ct.ThrowIfCancellationRequested();

		await table.Delete(userId);

		Invalidate(userId);
	}
}