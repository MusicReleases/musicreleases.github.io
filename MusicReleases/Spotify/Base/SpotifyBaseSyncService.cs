using JakubKastner.MusicReleases.BackgroundTasks.Enums;
using JakubKastner.MusicReleases.BackgroundTasks.Extensions;
using JakubKastner.MusicReleases.BackgroundTasks.Objects;
using JakubKastner.MusicReleases.BackgroundTasks.Services;
using JakubKastner.MusicReleases.Database.Spotify.BaseServices;
using JakubKastner.MusicReleases.Database.Spotify.Links;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify.User.Update;
using JakubKastner.SpotifyApi.Clients;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Spotify.Base;

internal abstract class SpotifyBaseSyncService<TModel, TPayload>
(
	ISpotifyUserClient userApi,
	ISpotifyReadByPayloadService<TModel, TPayload> reader,
	ISpotifyWriteEntityService<TModel> writer,
	ISpotifyUserLinkEntityService<TPayload> userLinkDbService,
	ISpotifyUserUpdateDbService updateDb,
	ISpotifyState<TModel> state,
	IBackgroundTaskManagerService taskManager,
	ILoadingService loadingService
)
	: SpotifyBaseSyncServiceCore<TModel, NoContext>(userApi, updateDb, taskManager, loadingService), ISpotifyBaseSyncService
	where TModel : SpotifyIdNameObject
	where TPayload : ISpotifyPayload
{

	private readonly ISpotifyReadByPayloadService<TModel, TPayload> _reader = reader;
	private readonly ISpotifyWriteEntityService<TModel> _writer = writer;
	private readonly ISpotifyUserLinkEntityService<TPayload> _userLinkDbService = userLinkDbService;
	private readonly ISpotifyState<TModel> _state = state;


	protected sealed override string GetTaskDescription(NoContext _) => TaskDescription;

	protected sealed override DateTime? GetLastSync(NoContext _) => LastSync;

	protected sealed override bool GetIsDataInState(NoContext _) => IsDataInState;


	protected abstract SpotifyDbUpdateType DbUpdateType { get; }

	protected abstract string TaskDescription { get; }

	protected abstract string EntityName { get; }

	protected abstract string UserLinkLabel { get; }

	protected abstract DateTime? LastSync { get; }

	protected abstract bool IsDataInState { get; }

	protected abstract Task<IReadOnlyCollection<TModel>> ApiLoad(CancellationToken ct);

	protected abstract TPayload CreatePayload(TModel model);

	protected virtual IReadOnlyCollection<TModel> MergePayloads(IReadOnlyCollection<TModel> models, IReadOnlyCollection<TPayload> payloads) => models;

	public Task Get(bool forceUpdate = false) => RunGet(default, forceUpdate);

	protected bool ShouldSync(bool forceUpdate) => ShouldSync(default, forceUpdate);

	protected sealed override async Task<bool> LoadFromDbToState(NoContext _, string userId, bool forceUpdate, BackgroundTask task)
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
				return await _reader.GetByIds(payloads, ct2);
			});

			return await task.RunSegment($"state - set {EntityName} - {count}", async _ =>
			{
				var merged = MergePayloads(models, payloads);
				SetState(models, lastSync);
				return ShouldSync(forceUpdate);
			});
		});
	}

	private void SetState(IReadOnlyCollection<TModel> models, DateTime lastSync)
	{
		_state.Set(models, lastSync);
	}

	protected sealed override async Task<IReadOnlyCollection<TModel>?> LoadFromApi(NoContext _, BackgroundTask task)
	{
		return await task.RunStep("Loading from API", BackgroundTaskCategory.GetApi, async ct =>
		{
			task.BeginAutoSegments(1);
			return await task.RunSegment($"api - get {EntityName}", ApiLoad);
		});
	}

	protected sealed override async Task SaveToDbAndState(NoContext _, IReadOnlyCollection<TModel> models, string userId, BackgroundTask task)
	{
		await task.RunStep("Saving to DB", BackgroundTaskCategory.SaveDb, async ct =>
		{
			task.BeginAutoSegments(4);
			var count = models.Count;

			await task.RunSegment($"db - save {EntityName} - {count}", async ct2 =>
			{
				await _writer.Save(models, true, ct2);
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