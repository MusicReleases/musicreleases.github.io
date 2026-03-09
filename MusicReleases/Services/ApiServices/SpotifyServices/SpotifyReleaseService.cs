using JakubKastner.MusicReleases.Database.Spotify.Entities;
using JakubKastner.MusicReleases.Database.Spotify.Mappers;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Objects.Spotify;
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

	public async Task Get(ReleaseGroup releaseType, bool forceUpdate = false)
	{
		if (releaseType == ReleaseGroup.Podcasts)
		{
			// TODO podcasts
			throw new NotSupportedException();
		}

		// cancel any ongoing sync
		Cancel();
		_cts = new CancellationTokenSource();
		var token = _cts.Token;

		try
		{
			var isInState = _state.ReleasesByType.GetValueOrDefault(releaseType) is not null;

			if (!isInState)
			{
				// load data from db to state
				await LoadFromDbToState(releaseType);
			}
			var lastSync = _state.LastSyncByType.GetValueOrDefault(releaseType);

			var shouldSync = forceUpdate || (DateTime.Now - lastSync).TotalHours > 24;

			if (shouldSync)
			{
				var userId = _spotifyApiUserService.GetUserIdRequired();

				await _taskManager.Run($"Getting {releaseType.ToFriendlyString()}", async (task) =>
				{
					await SyncProcess(userId, releaseType, task, token);
				});
			}
		}
		catch (OperationCanceledException)
		{
			// cancelled
		}
	}

	private async Task LoadFromDbToState(ReleaseGroup releaseType)
	{
		var artists = _artistState.SortedFollowedArtists;
		if (artists.Count == 0)
		{
			return;
		}

		var artistIds = artists.Select(a => a.Id);
		var artistRole = EnumReleaseTypeExtensions.MapReleaseRoleFromGroup(releaseType);
		var releaseIds = await _linkDb.GetReleaseIds(artistIds, artistRole);
		var releases = await _releaseDb.GetByIds(releaseIds, releaseType);

		Console.WriteLine("last sync get");

		var userId = _spotifyApiUserService.GetUserIdRequired();
		var metaDbType = MapToDbUpdateType(releaseType);
		var lastSync = await _metaDb.Get(userId, metaDbType);

		Console.WriteLine("last sync  - " + lastSync);

		_state.Set(releaseType, releases, lastSync);
	}

	private async Task SyncProcess(string userId, ReleaseGroup releaseType, SpotifyBackgroundTask task, CancellationToken ct)
	{
		// get artists from state
		var artists = _artistState.SortedFollowedArtists;
		if (artists.Count == 0)
		{
			return;
		}

		task.Status = $"Getting {releaseType.ToFriendlyString()} releases from api...";

		var allReleasesToSave = new List<SpotifyRelease>();
		var allLinksToSave = new List<SpotifyArtistReleaseEntity>();
		var allArtistsToSave = new HashSet<SpotifyArtist>();

		foreach (var artist in artists)
		{
			ct.ThrowIfCancellationRequested();

			// get releases from api
			var apiReleases = await _api.GetByArtist(artist, releaseType, ct);

			if (apiReleases.Count == 0)
			{
				continue;
			}

			// save release to db
			allReleasesToSave.AddRange(apiReleases);

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
		}

		ct.ThrowIfCancellationRequested();

		// save to db
		await _releaseDb.Save(allReleasesToSave);
		await _artistDb.Save(allArtistsToSave.ToList());
		await _linkDb.Save(allLinksToSave);

		// update meta db
		var metaDbType = MapToDbUpdateType(releaseType);
		await _metaDb.Save(userId, metaDbType);

		// update state
		task.Status = "Displaying releases...";

		_state.Set(releaseType, allReleasesToSave, DateTime.Now);
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
}