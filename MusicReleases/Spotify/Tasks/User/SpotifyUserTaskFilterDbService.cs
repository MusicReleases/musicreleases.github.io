using DexieNET;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Spotify.User;
using JakubKastner.SpotifyApi.User;
using System.Linq.Expressions;

namespace JakubKastner.MusicReleases.Spotify.Tasks.User;

internal class SpotifyUserTaskFilterDbService(IDbSpotifyService dbService, ISpotifyUserClient userClient) : SpotifyUserScopedEntityService<BackgroundTaskFilter, SpotifyUserFilterTaskEntity>(userClient), ISpotifyUserTaskFilterDbService
{
	private readonly IDbSpotifyService _dbService = dbService;

	protected override Expression<Func<SpotifyUserFilterTaskEntity, string>> UserIdExpression => x => x.UserId;

	protected override string GetEntityUserId(SpotifyUserFilterTaskEntity entity) => entity.UserId;

	protected override SpotifyUserFilterTaskEntity ToEntity(BackgroundTaskFilter model, string userId) => model.ToEntity(userId);

	protected override BackgroundTaskFilter ToModel(SpotifyUserFilterTaskEntity entity) => entity.ToModel();

	protected override async Task<Table<SpotifyUserFilterTaskEntity, string>> GetTable()
	{
		var db = await _dbService.GetDb();

		return db.UserFilterTask;
	}


	/*

	public async Task<TaskFilter?> Get(string userId)
	{
		var db = await _dbService.GetDb();

		var filterDb = await db.UserFilterTask.Get(userId);
		var filter = filterDb?.ToModel();

		return filter;
	}

	public async Task Save(TaskFilter filter, string userId)
	{
		var db = await _dbService.GetDb();
		var filterDb = filter.ToEntity(userId);

		await db.UserFilterTask.PutSafe(filterDb);
	}

	public async Task Delete(string userId)
	{
		var db = await _dbService.GetDb();

		await db.UserFilterTask.Delete(userId);
	}*/
}