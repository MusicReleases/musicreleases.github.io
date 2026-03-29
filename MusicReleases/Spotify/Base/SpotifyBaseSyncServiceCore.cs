using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify.Tasks;
using JakubKastner.MusicReleases.Spotify.User.Update;
using JakubKastner.SpotifyApi.Base.Objects;
using JakubKastner.SpotifyApi.User;

namespace JakubKastner.MusicReleases.Spotify.Base;

internal readonly struct NoContext { }

internal abstract class SpotifyBaseSyncServiceCore<TModel, TContext>(ISpotifyUserClient userApi, ISpotifyUserUpdateDbService updateDb, IBackgroundTaskManagerService taskManager, ILoadingService loadingService) where TModel : SpotifyIdNameObject
{
	private readonly ISpotifyUserClient _userApi = userApi;
	protected readonly ISpotifyUserUpdateDbService _updateDb = updateDb;
	private readonly IBackgroundTaskManagerService _taskManager = taskManager;
	private readonly ILoadingService _loadingService = loadingService;

	protected abstract BackgroundTaskType TaskType { get; }
	protected abstract string TaskTitle { get; }
	protected abstract string GetTaskDescription(TContext context);
	protected abstract DateTime? GetLastSync(TContext context);
	protected abstract bool GetIsDataInState(TContext context);

	protected bool ShouldSync(TContext context, bool forceUpdate)
	{
		if (forceUpdate)
		{
			return true;
		}
		return (DateTime.Now - (GetLastSync(context) ?? DateTime.MinValue)).TotalHours > 24;
	}

	protected abstract Task<bool> LoadFromDbToState(TContext context, string userId, bool forceUpdate, BackgroundTask task);
	protected abstract Task<IReadOnlyCollection<TModel>?> LoadFromApi(TContext context, string userId, BackgroundTask task, CancellationToken ct);
	protected abstract Task SaveToDbAndState(TContext context, IReadOnlyCollection<TModel> models, string userId, BackgroundTask task);



	protected async Task RunGet(TContext context, bool forceUpdate, BackgroundTaskSyncPlan? plan)
	{
		if (_loadingService.IsLoading(TaskType))
		{
			return;
		}

		var isInState = GetIsDataInState(context);
		if (isInState && !ShouldSync(context, forceUpdate))
		{
			return;
		}

		await _taskManager.Run(TaskType, TaskTitle, GetTaskDescription(context), async task =>
		{
			await RunGetInTask(context, forceUpdate, task, isInState, plan);
		});
	}
	protected async Task RunGetInExistingTask(TContext context, bool forceUpdate, BackgroundTask task, BackgroundTaskSyncPlan? plan)
	{
		if (_loadingService.IsLoading(TaskType))
		{
			return;
		}

		var isInState = GetIsDataInState(context);
		/*if (isInState && !ShouldSync(context, forceUpdate))
		{
			return;
		}
		*/
		await RunGetInTask(context, forceUpdate, task, isInState, plan);
	}


	private async Task RunGetInTask(TContext context, bool forceUpdate, BackgroundTask task, bool isInState, BackgroundTaskSyncPlan? plan)
	{
		var userId = _userApi.GetUserIdRequired();

		var shouldSync = !isInState || ShouldSync(context, forceUpdate);

		var dbStepId = plan?.DbStepId ?? Guid.NewGuid();
		var apiStepId = plan?.ApiStepId ?? Guid.NewGuid();
		var saveStepId = plan?.SaveStepId ?? Guid.NewGuid();

		IReadOnlyCollection<TModel>? apiData = null;

		await task.RunStep(dbStepId, "Loading from DB", BackgroundTaskCategory.GetDb, async ct =>
		{
			var step = task.CurrentStep;

			if (step is not null)
			{
				if (plan?.WaitBeforeDbStepId is Guid depDb)
				{
					await task.WaitForStep(step, depDb);
				}

				if (!shouldSync)
				{
					step.WasSkipped = true;
					step.SkipReason = "Loaded from store";
					return;
				}
			}
			shouldSync = await LoadFromDbToState(context, userId, forceUpdate, task);
		});

		await task.RunStep(apiStepId, "Loading from API", BackgroundTaskCategory.GetApi, async ct =>
		{
			var step = task.CurrentStep;

			if (step is not null)
			{
				if (plan?.WaitBeforeApiStepId is Guid depApi)
				{
					await task.WaitForStep(step, depApi);
				}

				if (!shouldSync)
				{
					step.WasSkipped = true;
					step.SkipReason = "No API sync needed";
					return;
				}
			}
			apiData = await LoadFromApi(context, userId, task, ct);
		});

		await task.RunStep(saveStepId, "Saving to DB", BackgroundTaskCategory.SaveDb, async ct =>
		{
			var step = task.CurrentStep;

			if (!shouldSync || apiData is null)
			{
				if (step is not null)
				{

					step.WasSkipped = true;
					step.SkipReason = "Nothing to save";
				}

				return;
			}

			await SaveToDbAndState(context, apiData, userId, task);
		});
	}
}
