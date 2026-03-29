using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Database.Spotify.Services.Links;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify.Tasks;
using JakubKastner.MusicReleases.Spotify.User.Update;
using JakubKastner.SpotifyApi.Base.Objects;
using JakubKastner.SpotifyApi.User;

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

	protected abstract IAsyncEnumerable<IReadOnlyCollection<TModel>> ApiLoadBatches(CancellationToken ct);

	protected abstract TPayload CreatePayload(TModel model);

	protected virtual IReadOnlyCollection<TModel> MergePayloads(IReadOnlyCollection<TModel> models, IReadOnlyCollection<TPayload> payloads) => models;

	public Task Get(bool forceUpdate = false) => RunGet(default, forceUpdate, null);

	protected bool ShouldSync(bool forceUpdate) => ShouldSync(default, forceUpdate);


	protected sealed override async Task<bool> LoadFromDbToState(NoContext _, string userId, bool forceUpdate, BackgroundTaskStep step)
	{
		var lastSync = await step.RunSegment
		(
			$"db - get {EntityName} last sync (user-update)",
			async ct =>
			{
				return await _updateDb.Get(userId, DbUpdateType, ct);
			}
		);

		var payloads = await step.RunSegment
		(
			$"db - get user {EntityName} ({UserLinkLabel})",
			async ct =>
			{
				return await _userLinkDbService.GetByUserId(userId, ct);
			}
		);

		var count = payloads.Count;

		if (count == 0)
		{
			await step.RunSegment
			(
				$"state - set {EntityName}",
				async _ =>
				{
					_state.Set([], lastSync);
				}
			);
			return true;
		}

		var models = await step.RunSegment
		(
			$"db - get {EntityName} by ids - {count}",
			async ct =>
			{
				return await _reader.GetByIds(payloads, ct);
			}
		);

		return await step.RunSegment
		(
			$"state - set {EntityName} - {count}",
			async _ =>
			{
				var merged = MergePayloads(models, payloads);
				_state.Set(models, lastSync);
				return ShouldSync(forceUpdate);
			}
		);
	}

	protected sealed override async Task LoadFromApi(NoContext _, string userId, BackgroundTaskStep step, CancellationToken ct)
	{
		var allPayloads = new List<TPayload>(capacity: 2048);

		// ----------------------------------- TODO count -----------------------------------
		//step.BeginAutoSegments(1);

		await foreach (var batch in ApiLoadBatches(ct).WithCancellation(ct))
		{
			ct.ThrowIfCancellationRequested();

			await step.RunSegment
			(
				$"db/state - save {EntityName} batch - {batch.Count}",
				async ct2 =>
				{
					await _writer.Save(batch, true, ct2);
					_state.AddRange(batch, DateTime.Now, false);
				}
			);

			allPayloads.AddRange(batch.Select(CreatePayload));

			// report progress

			await Task.Yield();
		}

		await step.RunSegment
		(
			$"db - save user {EntityName} ({UserLinkLabel}) - {allPayloads.Count}",
			async ct2 =>
			{
				await _userLinkDbService.SaveByUserId(userId, allPayloads, ct2);
			}
		);

		// save last sync
		await step.RunSegment
		(
			$"db - save {EntityName} last sync (update)",
			async ct2 =>
			{
				await _updateDb.Save(userId, DbUpdateType, ct2);
			}
		);
	}

	/*protected sealed override async Task SaveToDbAndState(NoContext _, IReadOnlyCollection<TModel> models, string userId, BackgroundTaskStep step)
	{
		//step.BeginAutoSegments(4);
		var count = models.Count;

		await step.RunSegment
		(
			$"db - save {EntityName} - {count}",
			async ct2 =>
			{
				await _writer.Save(models, true, ct2);
			}
		);

		await step.RunSegment
		(
			$"db - save user {EntityName} ({UserLinkLabel}) - {count}",
			async ct2 =>
			{
				var payloads = models.Select(CreatePayload);

				await _userLinkDbService.SaveByUserId(userId, payloads, ct2);
			}
		);

		await step.RunSegment
		(
			$"db - save {EntityName} last sync (update)",
			async ct2 =>
			{
				await _updateDb.Save(userId, DbUpdateType, ct2);
			}
		);

		await step.RunSegment
		(
			$"state - set {EntityName} - {count}",
			async _ =>
			{
				_state.ReconcileSnapshot(models, DateTime.Now);
			}
		);
	}*/
}