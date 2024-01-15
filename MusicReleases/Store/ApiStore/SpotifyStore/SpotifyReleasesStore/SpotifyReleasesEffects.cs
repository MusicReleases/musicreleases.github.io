using Blazored.LocalStorage;
using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;
using JakubKastner.SpotifyApi.Controllers;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasestore;

public class SpotifyReleasesEffects(ISpotifyControllerRelease spotifyControllerRelease, ILocalStorageService localStorageService, IState<SpotifyReleasesState> releasesState)
{
	private readonly ISpotifyControllerRelease _spotifyControllerRelease = spotifyControllerRelease;
	private readonly ILocalStorageService _localStorageService = localStorageService;
	private readonly IState<SpotifyReleasesState> _releasesState = releasesState;

	private const string _localStorageName = "spotify_releases";

	[EffectMethod]
	public async Task LoadReleases(SpotifyReleasesActionLoad action, IDispatcher dispatcher)
	{
		try
		{
			var releases = await _spotifyControllerRelease.GetAllUserFollowedArtistsReleases(action.ReleaseType);
			dispatcher.Dispatch(new SpotifyReleasesActionSet(releases));
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyReleasesActionLoadFailure(ex.Message));
		}
	}

	// local storage:
	[EffectMethod]
	public async Task SetStorage(SpotifyReleasesActionStorageSet action, IDispatcher dispatcher)
	{
		try
		{
			// set item
			await _localStorageService.SetItemAsync(_localStorageName, action.ReleasesState);

			dispatcher.Dispatch(new SpotifyReleasesActionStorageSetSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyReleasesActionStorageSetFailure(ex.Message));
		}
	}

	[EffectMethod(typeof(SpotifyReleasesActionStorageGet))]
	public async Task LoadStorage(IDispatcher dispatcher)
	{
		try
		{
			// get item
			var state = await _localStorageService.GetItemAsync<SpotifyReleasesState>(_localStorageName);

			if (state is not null)
			{
				dispatcher.Dispatch(new SpotifyReleasesActionStorageSet(state));
				dispatcher.Dispatch(new SpotifyReleasesActionStorageGetSuccess());
			}
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyReleasesActionStorageGetFailure(ex.Message));
		}
	}

	[EffectMethod(typeof(SpotifyReleasesActionStorageClear))]
	public async Task ClearStorage(IDispatcher dispatcher)
	{
		try
		{
			// remove item
			await _localStorageService.RemoveItemAsync(_localStorageName);

			dispatcher.Dispatch(new SpotifyReleasesActionStorageSet(new()
			{
				Initialized = false,
				Loading = false,
				Releases = [],
			}));
			dispatcher.Dispatch(new SpotifyReleasesActionStorageClearSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyReleasesActionStorageClearFailure(ex.Message));
		}
	}
}
