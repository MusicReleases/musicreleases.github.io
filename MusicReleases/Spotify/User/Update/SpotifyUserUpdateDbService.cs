using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;

namespace JakubKastner.MusicReleases.Spotify.User.Update;

public class SpotifyUserUpdateDbService(IDbSpotifyService dbService) : ISpotifyUserUpdateDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	private async Task<Table<SpotifyUserUpdateEntity, string>> GetTable()
	{
		var db = await _dbService.GetDb();
		return db.UserUpdate;
	}

	public async Task<DateTime> Get(string userId, SpotifyDbUpdateType updateType, CancellationToken ct = default)
	{
		var key = SpotifyUserUpdateEntity.MakeKey(userId, updateType);

		var table = await GetTable();

		ct.ThrowIfCancellationRequested();

		var meta = await table.Get(key);

		var lastUpdate = meta?.LastUpdate ?? DateTime.MinValue;
		return lastUpdate;
	}

	public async Task Save(string userId, SpotifyDbUpdateType updateType, CancellationToken ct = default)
	{
		var entity = userId.ToSpotifyUpdateEntity(updateType);

		var table = await GetTable();

		ct.ThrowIfCancellationRequested();

		await table.PutSafe(entity);
	}

	public async Task Delete(string userId, SpotifyDbUpdateType updateType)
	{
		var key = SpotifyUserUpdateEntity.MakeKey(userId, updateType);

		var table = await GetTable();
		await table.Delete(key);
	}

	public async Task DeleteForUser(string userId)
	{
		var keys = EnumExtensions.GetValues<SpotifyDbUpdateType>().Select(updateType => SpotifyUserUpdateEntity.MakeKey(userId, updateType)).ToArray();

		var table = await GetTable();
		await table.BulkDeleteSafe(keys);
	}
}