using JakubKastner.MusicReleases.BackgroundTasks.Enums;
using JakubKastner.MusicReleases.BackgroundTasks.Extensions;
using JakubKastner.MusicReleases.BackgroundTasks.Objects;
using JakubKastner.MusicReleases.BackgroundTasks.Services;
using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Entities.Base;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify;
using JakubKastner.SpotifyApi.Clients;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Services;

internal abstract class SpotifyBaseSyncService<TModel, TEntity, TUserLinkEntity, TPayload>(ISpotifyUserClient userApi, /*ISpotifyPlaylistClient playlistApi,*/ ISpotifyEntityService<TModel, TPayload> entityDbService, ISpotifyUserRelationService<TPayload> userLinkDbService, IDbSpotifyUserUpdateService updateDb, ISpotifyState<TModel> state, IBackgroundTaskManagerService taskManager, ILoadingService loadingService) : ISpotifyBaseSyncService where TModel : SpotifyIdNameObject
	where TEntity : ISpotifyIdEntity
	where TUserLinkEntity : ISpotifyUserIdEntity
	where TPayload : ISpotifyPayload
{
	private readonly ISpotifyUserClient _userApi = userApi;
	//private readonly ISpotifyArtistClient _artistApi = artistApi;
	private readonly ISpotifyEntityService<TModel, TPayload> _entityDbService = entityDbService;
	private readonly ISpotifyUserRelationService<TPayload> _userLinkDbService = userLinkDbService;
	private readonly IDbSpotifyUserUpdateService _updateDb = updateDb;
	private readonly ISpotifyState<TModel> _state = state;
	private readonly IBackgroundTaskManagerService _taskManager = taskManager;
	private readonly ILoadingService _loadingService = loadingService;

	protected abstract BackgroundTaskType TaskType { get; }

	protected abstract SpotifyDbUpdateType DbUpdateType { get; }

	protected abstract string TaskTitle { get; }

	protected abstract string TaskDescription { get; }

	protected abstract string EntityName { get; }

	protected abstract string UserLinkLabel { get; }

	protected abstract DateTime? LastSync { get; }

	protected abstract bool IsDataInState { get; }

	protected abstract Task<IReadOnlyCollection<TModel>> ApiLoad(CancellationToken ct);

	protected abstract TPayload CreatePayload(TModel model);

	protected virtual IReadOnlyCollection<TModel> MergePayloads(IReadOnlyCollection<TModel> models, IReadOnlyCollection<TPayload> payloads) => models;

	protected bool ShouldSync(bool forceUpdate)
	{
		if (forceUpdate)
		{
			return true;
		}
		return (DateTime.Now - (LastSync ?? DateTime.MinValue)).TotalHours > 24;
	}

	public async Task Get(bool forceUpdate = false)
	{
		if (_loadingService.IsLoading(TaskType))
		{
			return;
		}

		var isInState = IsDataInState;

		if (isInState && !ShouldSync(forceUpdate))
		{
			return;
		}

		await _taskManager.Run(TaskType, TaskTitle, TaskDescription, async task =>
		{
			var userId = _userApi.GetUserIdRequired();

			if (!isInState)
			{
				var shouldSync = await LoadFromDbToState(userId, forceUpdate, task);
				if (!shouldSync) return;
			}

			var apiData = await LoadFromApi(task);
			await SaveToDbAndState(apiData, userId, task);
		});
	}

	private async Task<bool> LoadFromDbToState(string userId, bool forceUpdate, BackgroundTask task)
	{
		return await task.RunStep("Loading from DB", BackgroundTaskCategory.GetDb, async ct =>
		{
			task.BeginAutoSegments(5);

			var lastSync = await task.RunSegment($"db - get {EntityName} last sync (user-update)", async ct2 =>
			{
				return await _updateDb.Get(userId, DbUpdateType, ct2);
			});

			var payloads = await task.RunSegment($"db - get user {EntityName} ({UserLinkLabel})", async ct2 =>
			{
				return await _userLinkDbService.GetByUserId(userId, ct2);
			});

			var count = payloads.Count;

			if (count == 0)
			{
				await task.RunSegment($"state - set {EntityName}", async _ =>
				{
					SetState([], lastSync);
				});
				return true;
			}

			var models = await task.RunSegment($"db - get {EntityName} by ids - {count}", async ct2 =>
			{
				return await _entityDbService.GetByIds(payloads, ct2);
			});

			return await task.RunSegment($"state - set {EntityName} - {count}", (Func<CancellationToken, Task<bool>>)(async _ =>
			{
				var merged = MergePayloads(models, payloads);
				SetState(models, lastSync);
				return ShouldSync(forceUpdate);
			}));
		});
	}

	private void SetState(IReadOnlyCollection<TModel> models, DateTime lastSync)
	{
		_state.Set(models, lastSync);
	}


	private async Task<IReadOnlyCollection<TModel>> LoadFromApi(BackgroundTask task)
	{
		return await task.RunStep("Loading from API", BackgroundTaskCategory.GetApi, async ct =>
		{
			task.BeginAutoSegments(1);
			return await task.RunSegment($"api - get {EntityName}", ApiLoad);
		});
	}

	private async Task SaveToDbAndState(IReadOnlyCollection<TModel> models, string userId, BackgroundTask task)
	{
		await task.RunStep("Saving to DB", BackgroundTaskCategory.SaveDb, async ct =>
		{
			task.BeginAutoSegments(4);
			var count = models.Count;

			await task.RunSegment($"db - save {EntityName} - {count}", async ct2 =>
			{
				await _entityDbService.Save(models, ct2);
			});

			await task.RunSegment($"db - save user {EntityName} ({UserLinkLabel}) - {count}", async ct2 =>
			{
				var payloads = models.Select(CreatePayload);

				await _userLinkDbService.SaveByUserId(userId, payloads, ct2);
			});

			await task.RunSegment($"db - save {EntityName} last sync (update)", async ct2 =>
			{
				await _updateDb.Save(userId, DbUpdateType, ct2);
			});

			await task.RunSegment($"state - set {EntityName} - {count}", async _ =>
			{
				SetState(models, DateTime.Now);
			});
		});
	}
}