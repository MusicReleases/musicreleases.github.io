using JakubKastner.MusicReleases.BackgroundTasks.Enums;
using JakubKastner.MusicReleases.BackgroundTasks.Objects;
using JakubKastner.MusicReleases.BackgroundTasks.Services;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify.User.Update;
using JakubKastner.SpotifyApi.Clients;
using JakubKastner.SpotifyApi.Objects.Base;

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
	protected abstract Task<IReadOnlyCollection<TModel>?> LoadFromApi(TContext context, BackgroundTask task);
	protected abstract Task SaveToDbAndState(TContext context, IReadOnlyCollection<TModel> models, string userId, BackgroundTask task);

	protected async Task RunGet(TContext context, bool forceUpdate)
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
			var userId = _userApi.GetUserIdRequired();

			if (!isInState)
			{
				var shouldSync = await LoadFromDbToState(context, userId, forceUpdate, task);
				if (!shouldSync)
				{
					return;
				}
			}

			var apiData = await LoadFromApi(context, task);
			if (apiData is null)
			{
				return;
			}

			await SaveToDbAndState(context, apiData, userId, task);
		});
	}
}
