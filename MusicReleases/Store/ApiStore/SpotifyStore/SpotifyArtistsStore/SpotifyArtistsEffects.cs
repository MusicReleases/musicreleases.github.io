using Blazored.LocalStorage;
using Fluxor;
using JakubKastner.SpotifyApi.Controllers;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;

public class SpotifyArtistsEffects(ISpotifyControllerArtist spotifyControllerArtist, ILocalStorageService localStorageService, IState<SpotifyArtistsState> artistsState)
{
	private readonly ISpotifyControllerArtist _spotifyControllerArtist = spotifyControllerArtist;
	private readonly ILocalStorageService _localStorageService = localStorageService;
	private readonly IState<SpotifyArtistsState> _artistsState = artistsState;

	private const string _localStorageName = "spotify_artists";

	[EffectMethod(typeof(SpotifyArtistsActionLoad))]
	public async Task LoadArtists(IDispatcher dispatcher)
	{
		try
		{
			var artists = await _spotifyControllerArtist.GetUserFollowedArtists();
			dispatcher.Dispatch(new SpotifyArtistsActionSet(artists));
			dispatcher.Dispatch(new SpotifyArtistsActionLoadSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyArtistsActionLoadFailure(ex.Message));
		}
	}

	/* --> TODO from another
	 * 
	 * [EffectMethod(typeof(CounterIncrementAction))]
	public async Task LoadForecastsOnIncrement(IDispatcher dispatcher)
	{
		await Task.Delay(0);
		if (CounterState.Value.CurrentCount % 10 == 0)
		{
			dispatcher.Dispatch(new WeatherLoadForecastsAction());
		}
	}*/

	// local storage:

	// PersistState
	[EffectMethod]
	public async Task SetStorage(SpotifyArtistsActionStorageSet action, IDispatcher dispatcher)
	{
		try
		{
			// set item
			await _localStorageService.SetItemAsync(_localStorageName, action.ArtistsState);

			dispatcher.Dispatch(new SpotifyArtistsActionStorageSetSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyArtistsActionStorageSetFailure(ex.Message));
		}
	}

	[EffectMethod(typeof(SpotifyArtistsActionStorageGet))]
	public async Task LoadStorage(IDispatcher dispatcher)
	{
		try
		{
			// get item
			var artistsState = await _localStorageService.GetItemAsync<SpotifyArtistsState>(_localStorageName);

			if (artistsState is not null)
			{
				dispatcher.Dispatch(new SpotifyArtistsActionStorageSet(artistsState));
				dispatcher.Dispatch(new SpotifyArtistsActionStorageGetSuccess());
			}
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyArtistsActionStorageGetFailure(ex.Message));
		}
	}

	[EffectMethod(typeof(SpotifyArtistsActionStorageClear))]
	public async Task ClearStorage(IDispatcher dispatcher)
	{
		try
		{
			// remove item
			await _localStorageService.RemoveItemAsync(_localStorageName);

			dispatcher.Dispatch(new SpotifyArtistsActionStorageSet(new()
			{
				Initialized = false,
				Loading = false,
				Artists = [],
			}));
			dispatcher.Dispatch(new SpotifyArtistsActionStorageClearSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyArtistsActionStorageClearFailure(ex.Message));
		}
	}
}
