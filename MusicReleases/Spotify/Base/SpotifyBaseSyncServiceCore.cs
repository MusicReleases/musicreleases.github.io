using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify.Tasks;
using JakubKastner.MusicReleases.Spotify.User.Update;
using JakubKastner.SpotifyApi.Base.Objects;
using JakubKastner.SpotifyApi.User;

namespace JakubKastner.MusicReleases.Spotify.Base;

internal readonly struct NoContext { }

internal abstract class SpotifyBaseSyncServiceCore<TModel, TContext>
(
	ISpotifyUserClient userApi,
	ISpotifyUserUpdateDbService updateDb,
	IBackgroundTaskManagerService taskManager,
	ILoadingService loadingService
)
where TModel : SpotifyIdNameObject
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

	protected abstract Task<bool> LoadFromDbToState(TContext context, string userId, bool forceUpdate, BackgroundTaskStep step);
	protected abstract Task LoadFromApi(TContext context, string userId, BackgroundTaskStep step, CancellationToken ct);
	//protected abstract Task SaveToDbAndState(TContext context, IReadOnlyCollection<TModel> models, string userId, BackgroundTaskStep step);



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

		await task.RunStep
		(
			plan?.DbStepId ?? Guid.NewGuid(),
			BackgroundTaskCategory.GetDb,
			async (ct, step) =>
			{
				if (plan?.WaitBeforeDbStepId is Guid depDb)
				{
					await task.WaitForStep(step, depDb);
				}

				if (!shouldSync)
				{
					step.MarkSkipped("Loaded from store");
					return;
				}

				shouldSync = await LoadFromDbToState(context, userId, forceUpdate, step);
			}
		);

		//IReadOnlyCollection<TModel>? apiData = null;

		await task.RunStep
		(
			plan?.ApiStepId ?? Guid.NewGuid(),
			BackgroundTaskCategory.GetApi,
			async (ct, step) =>
			{
				if (plan?.WaitBeforeApiStepId is Guid depApi)
				{
					await task.WaitForStep(step, depApi);
				}

				if (!shouldSync)
				{
					step.MarkSkipped("No API sync needed");
					return;
				}

				await LoadFromApi(context, userId, step, ct);
				//apiData = await LoadFromApi(context, userId, step, ct);
			}
		);

		/*await task.RunStep
		(
			plan?.SaveStepId ?? Guid.NewGuid(),
			BackgroundTaskCategory.SaveDb,
			async (ct, step) =>
			{
				if (!shouldSync || apiData is null)
				{
					step.MarkSkipped("Nothing to save");
					return;
				}

				await SaveToDbAndState(context, apiData, userId, step);
			}
		);*/
	}
}
