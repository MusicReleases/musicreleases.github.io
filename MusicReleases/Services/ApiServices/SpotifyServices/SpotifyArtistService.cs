using JakubKastner.MusicReleases.Objects.Spotify;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Services.Api;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyArtistService(ISpotifyFilterService filterService, IApiArtistService api, IDbSpotifyArtistService artistDb, IDbSpotifyUserArtistService linkDb, IDbSpotifyUpdateService metaDb, ISpotifyArtistState state, ISpotifyTaskManagerService taskManager) : ISpotifyArtistService
{
	private readonly ISpotifyFilterService _filterService = filterService;
	private readonly IApiArtistService _api = api;
	private readonly IDbSpotifyArtistService _artistDb = artistDb;
	private readonly IDbSpotifyUserArtistService _linkDb = linkDb;
	private readonly IDbSpotifyUpdateService _metaDb = metaDb;
	private readonly ISpotifyArtistState _state = state;
	private readonly ISpotifyTaskManagerService _taskManager = taskManager;

	private CancellationTokenSource? _cts;

	public async Task Get(string userId, bool forceUpdate = false)
	{
		// 1. Zrušit předchozí běh (pokud uživatel kliká zběsile)
		Cancel();
		_cts = new CancellationTokenSource();
		var token = _cts.Token;

		try
		{
			await LoadFromDbToState(userId);

			// KROK B: Rozhodnutí o syncu (Smart Logic)
			// Nahrazuje tvoji logiku "GetUserFollowedArtists(artistsDb, forceUpdate)"
			var lastSync = await _metaDb.Get(userId, LoadingType.Artists);
			var shouldSync = forceUpdate || (DateTime.Now - lastSync).TotalHours > 24;

			if (shouldSync)
			{
				// Spustíme dlouhou operaci na pozadí s vizualizací v TaskManageru
				await _taskManager.Run("Synchronizace Umělců", async (task) =>
				{
					await SyncProcess(userId, task, token);
				});
			}
		}
		catch (OperationCanceledException)
		{
			// Sync zrušen, nic se neděje
		}
	}

	// Pomocná metoda pro načtení z DB do UI
	private async Task LoadFromDbToState(string userId)
	{
		// Získáme IDčka
		var ids = await _linkDb.GetFollowedIds(userId);

		if (ids.Count > 0)
		{
			var artists = await _artistDb.GetByIds(ids);

			_state.SetFollowed(artists);

			_filterService.SetArtists(_state.SortedFollowedArtists.ToHashSet());
		}
	}

	// Samotný proces synchronizace (API -> DB -> UI)
	private async Task SyncProcess(string userId, SpotifyBackgroundTask task, CancellationToken ct)
	{
		// 1. API STAHIVÁNÍ
		task.Status = "Stahuji seznam ze Spotify...";
		// Tady voláš API. Pokud API vrací "nic nového" (304 Not Modified), můžeš skončit.
		// Ale pro jednoduchost stahujeme vše.
		var apiArtists = await _api.GetFollowed(ct);
		ct.ThrowIfCancellationRequested();

		// 2. UKLÁDÁNÍ DO DB (Batch)
		task.Status = $"Ukládám {apiArtists.Count} umělců do DB...";

		// A) Uložit data artistů (Upsert - aktualizuje jména/fotky)
		await _artistDb.Save(apiArtists);
		ct.ThrowIfCancellationRequested();

		// B) Aktualizovat vazby (Kdo koho sleduje) - TOHLE JE TA DELTA LOGIKA
		var apiIds = apiArtists.Select(a => a.Id);
		await _linkDb.SetFollowed(userId, apiIds);

		// C) Uložit čas syncu
		await _metaDb.SetLastArtistSync(userId, LoadingType.Artists);

		// 3. AKTUALIZACE UI (Finální refresh)
		task.Status = "Aktualizuji zobrazení...";
		_state.SetFollowed(apiArtists);

		_filterService.SetArtists(_state.SortedFollowedArtists.ToHashSet());
	}

	public void Cancel()
	{
		_cts?.Cancel();
	}
}