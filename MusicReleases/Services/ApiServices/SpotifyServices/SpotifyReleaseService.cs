using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.Spotify;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services;
using JakubKastner.SpotifyApi.Services.Api;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyReleaseService(ISpotifyApiUserService spotifyApiUserService, IApiReleaseClient api, IDbSpotifyReleaseService releaseDb, IDbSpotifyArtistService artistDb, IDbSpotifyArtistReleaseService linkDb, IDbSpotifyUserUpdateService metaDb, ISpotifyArtistState artistState, ISpotifyReleaseState state, ISpotifyTaskManagerService taskManager) : ISpotifyReleaseService
{
	private readonly ISpotifyApiUserService _spotifyApiUserService = spotifyApiUserService;
	private readonly IApiReleaseClient _api = api;
	private readonly IDbSpotifyReleaseService _releaseDb = releaseDb;
	private readonly IDbSpotifyArtistService _artistDb = artistDb;
	private readonly IDbSpotifyArtistReleaseService _linkDb = linkDb;
	private readonly IDbSpotifyUserUpdateService _metaDb = metaDb;
	private readonly ISpotifyArtistState _artistState = artistState;
	private readonly ISpotifyReleaseState _state = state;
	private readonly ISpotifyTaskManagerService _taskManager = taskManager;

	private CancellationTokenSource? _cts;

	public async Task Get(ReleaseGroup releaseGroup, bool forceUpdate = false)
	{
		if (releaseGroup == ReleaseGroup.Podcasts)
		{
			// TODO podcasts
			throw new NotSupportedException();
		}

		// cancel any ongoing sync
		Cancel();
		_cts = new CancellationTokenSource();
		var ct = _cts.Token;

		try
		{
			await _taskManager.Run($"Geting {releaseGroup.ToFriendlyString()}", BackgroundTaskType.Releases,
				async task =>
				{
					var userId = _spotifyApiUserService.GetUserIdRequired();

					var isInState = _state.ReleasesByType.GetValueOrDefault(releaseGroup) is not null;

					if (!isInState)
					{
						// load data from db to state
						await using (await task.BeginStepAsync("Loading from DB", BackgroundTaskCategory.GetDb))
						{
							await LoadFromDbToState(releaseGroup, userId, task, ct);
						}
					}

					// calculate last sync
					var lastSync = _state.LastSyncByType.GetValueOrDefault(releaseGroup);
					var shouldSync = forceUpdate || (DateTime.Now - lastSync).TotalHours > 24;

					if (!shouldSync)
					{
						// end task (synced)
						return;
					}

					// load from api
					ReleaseAggregation releaseAggregation;
					await using (await task.BeginStepAsync("Loading from API", BackgroundTaskCategory.GetApi))
					{
						releaseAggregation = await LoadFromApi(releaseGroup, task, ct);
					}

					// save api data to db and state
					await using (await task.BeginStepAsync("Saving to DB", BackgroundTaskCategory.SaveDb))
					{
						await SaveToDbAndState(releaseGroup, releaseAggregation, userId, task, ct);
					}
				});
		}
		catch (OperationCanceledException)
		{
			// cancelled
		}
	}


	private async Task LoadFromDbToState(ReleaseGroup releaseGroup, string userId, BackgroundTask task, CancellationToken ct)
	{
		task.SetSubProgress(0.00, "db.get.last-sync");
		var metaDbType = MapToDbUpdateType(releaseGroup);
		var lastSync = await _metaDb.Get(userId, metaDbType);

		var artists = _artistState.SortedFollowedArtists;
		if (artists.Count == 0)
		{
			_state.Set(releaseGroup, [], lastSync);
			task.SetSubProgress(1.00, "empty");
			return;
		}

		var artistIds = artists.Select(a => a.Id);
		var artistRole = EnumReleaseTypeExtensions.MapReleaseRoleFromGroup(releaseGroup);

		task.SetSubProgress(0.20, "db.get.artist-releases.by-artists");
		var releaseIds = await _linkDb.GetReleaseIds(artistIds, artistRole);

		task.SetSubProgress(0.60, "db.get.releases.by-ids");
		var releases = await _releaseDb.GetByIds(releaseIds, releaseGroup);

		_state.Set(releaseGroup, releases, lastSync);
		task.SetSubProgress(1.00, "db.get.releases.done");
	}

	private async Task<ReleaseAggregation> LoadFromApi(ReleaseGroup releaseGroup, BackgroundTask task, CancellationToken ct)
	{
		// get artists from state
		task.SetSubProgress(0.00, "api.get.releases.start");
		var artists = _artistState.SortedFollowedArtists;
		if (artists.Count == 0)
		{
			task.SetSubProgress(1.00, "no-artists");
			return new([], [], []);
		}

		//task.Status = $"Getting {releaseType.ToFriendlyString()} releases from api...";

		var allReleasesToSave = new HashSet<SpotifyRelease>();
		var allLinksToSave = new HashSet<SpotifyArtistReleaseEntity>();
		var allArtistsToSave = new HashSet<SpotifyArtist>();


		var total = artists.Count;
		var i = 0;


		foreach (var artist in artists)
		{
			ct.ThrowIfCancellationRequested();

			var frac = (double)i / Math.Max(1, total);
			task.SetSubProgress(frac, $"api.get.releases.artist-{i + 1}/{total}");

			// get releases from api
			var apiReleases = await _api.GetByArtist(artist, releaseGroup, ct);

			if (apiReleases.Count == 0)
			{
				continue;
			}

			// save release to db
			allReleasesToSave.UnionWith(apiReleases);

			// save artists and links to db
			foreach (var release in apiReleases)
			{
				foreach (var releaseArtist in release.Artists)
				{
					allArtistsToSave.Add(releaseArtist);
					allLinksToSave.Add(release.Id.ToArtistReleaseEntity(releaseArtist.Id, ArtistReleaseRole.Main));
				}

				foreach (var featArtist in release.FeaturedArtists)
				{
					allArtistsToSave.Add(featArtist);
					allLinksToSave.Add(release.Id.ToArtistReleaseEntity(featArtist.Id, ArtistReleaseRole.Featured));
				}
			}
			i++;
		}

		task.SetSubProgress(0.00, $"api.get.releases.done.total-{allReleasesToSave.Count}");
		return new(allReleasesToSave.ToList(), allArtistsToSave.ToList(), allLinksToSave.ToList());
	}

	private async Task SaveToDbAndState(ReleaseGroup releaseGroup, ReleaseAggregation releaseAggregation, string userId, BackgroundTask task, CancellationToken ct)
	{
		// save to db
		task.SetSubProgress(0.10, "db.save.releases");
		await _releaseDb.Save(releaseAggregation.Releases);

		task.SetSubProgress(0.35, "db.save.artists");
		await _artistDb.Save(releaseAggregation.Artists);

		task.SetSubProgress(0.60, "db.save.user-playlists");
		await _linkDb.Save(releaseAggregation.Links);

		// update meta db
		task.SetSubProgress(0.90, "db.save.last-sync");
		var metaDbType = MapToDbUpdateType(releaseGroup);
		await _metaDb.Save(userId, metaDbType);

		// update state

		_state.Set(releaseGroup, releaseAggregation.Releases, DateTime.Now);
		task.SetSubProgress(1.00, "db.save.releases.done");
	}

	private static SpotifyDbUpdateType MapToDbUpdateType(ReleaseGroup releasesType) => releasesType switch
	{
		ReleaseGroup.Albums => SpotifyDbUpdateType.ReleasesAlbums,
		ReleaseGroup.Tracks => SpotifyDbUpdateType.ReleasesTracks,
		ReleaseGroup.Appears => SpotifyDbUpdateType.ReleasesAppears,
		ReleaseGroup.Compilations => SpotifyDbUpdateType.ReleasesCompilations,
		ReleaseGroup.Podcasts => throw new NotSupportedException(),
		_ => throw new NotSupportedException(nameof(releasesType))
	};

	public void Cancel()
	{
		_cts?.Cancel();
	}

	private sealed record ReleaseAggregation(
		List<SpotifyRelease> Releases,
		List<SpotifyArtist> Artists,
		List<SpotifyArtistReleaseEntity> Links
	);

}