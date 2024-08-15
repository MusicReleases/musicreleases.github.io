using Blazored.LocalStorage;
using Fluxor;
using JakubKastner.SpotifyApi.Controllers;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

public class SpotifyPlaylistsEffects(ISpotifyControllerPlaylist spotifyControllerPlaylist, ILocalStorageService localStorageService, IState<SpotifyPlaylistsState> playlistsState)
{
	private const ServiceType serviceType = ServiceType.Spotify;

	private readonly string _localStorageName = GetLocalStorageKey(serviceType, LocalStorageKey.UserPlaylists);
	private readonly string _localStorageStateName = GetLocalStorageKey(serviceType, LocalStorageKey.UserPlaylistsState);

	private readonly ISpotifyControllerPlaylist _spotifyControllerPlaylist = spotifyControllerPlaylist;
	private readonly ILocalStorageService _localStorageService = localStorageService;

	private readonly IState<SpotifyPlaylistsState> _playlistsState = playlistsState;

	// GET
	[EffectMethod]
	public async Task Get(SpotifyPlaylistsActionGet action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		dispatcher.Dispatch(new SpotifyPlaylistsActionGetStorage(action.ForceUpdate));
	}

	[EffectMethod]
	public async Task GetStorage(SpotifyPlaylistsActionGetStorage action, IDispatcher dispatcher)
	{
		try
		{
			// get item from storage
			var playlists = await _localStorageService.GetItemAsync<SpotifyUserList<SpotifyPlaylist>>(_localStorageName);

			if (playlists is not null)
			{
				dispatcher.Dispatch(new SpotifyPlaylistsActionSet(playlists));
			}
			dispatcher.Dispatch(new SpotifyPlaylistsActionGetStorageSuccess());
			dispatcher.Dispatch(new SpotifyPlaylistsActionGetApi(playlists, action.ForceUpdate));
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionGetStorageFailure(ex.Message));
		}
	}

	[EffectMethod]
	public async Task GetApi(SpotifyPlaylistsActionGetApi action, IDispatcher dispatcher)
	{
		try
		{
			// get item from api
			var playlistsStorage = action.Playlists;
			var playlists = await _spotifyControllerPlaylist.GetUserPlaylists(true, playlistsStorage, action.ForceUpdate);
			dispatcher.Dispatch(new SpotifyPlaylistsActionGetApiSuccess());

			dispatcher.Dispatch(new SpotifyPlaylistsActionSet(playlists));
			dispatcher.Dispatch(new SpotifyPlaylistsActionSetStorage(playlists));
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionGetApiFailure(ex.Message));
		}
	}


	// SET
	[EffectMethod]
	public async Task SetStorage(SpotifyPlaylistsActionSetStorage action, IDispatcher dispatcher)
	{
		try
		{
			// set item
			await _localStorageService.SetItemAsync(_localStorageName, action.Playlists);

			dispatcher.Dispatch(new SpotifyPlaylistsActionSetStorageSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionSetStorageFailure(ex.Message));
		}
	}


	// TODO PERSIST STATE

	// TODO persist state when failed ??
	[EffectMethod(typeof(SpotifyPlaylistsActionGetApiFailure))]
	public async Task GetApiFailure(IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		dispatcher.Dispatch(new SpotifyPlaylistsActionSetStorageState(_playlistsState.Value));
	}

	[EffectMethod(typeof(SpotifyPlaylistsActionGetStorageState))]
	public async Task GetStorageState(IDispatcher dispatcher)
	{
		try
		{
			// get item
			var playlistsState = await _localStorageService.GetItemAsync<SpotifyPlaylistsState>(_localStorageStateName);

			if (playlistsState is not null)
			{
				dispatcher.Dispatch(new SpotifyPlaylistsActionSetStorageState(playlistsState));
				dispatcher.Dispatch(new SpotifyPlaylistsActionGetStorageStateSuccess());
			}
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionGetStorageStateFailure(ex.Message));
		}
	}

	[EffectMethod]
	public async Task SetStorageState(SpotifyPlaylistsActionSetStorageState action, IDispatcher dispatcher)
	{
		try
		{
			// set item
			await _localStorageService.SetItemAsync(_localStorageStateName, action.PlaylistsState);

			dispatcher.Dispatch(new SpotifyPlaylistsActionSetStorageStateSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionSetStorageStateFailure(ex.Message));
		}
	}

	[EffectMethod(typeof(SpotifyPlaylistsActionClearStorageState))]
	public async Task ClearStorageState(IDispatcher dispatcher)
	{
		try
		{
			// remove item
			await _localStorageService.RemoveItemAsync(_localStorageStateName);

			dispatcher.Dispatch(new SpotifyPlaylistsActionSetStorageState(new()));
			dispatcher.Dispatch(new SpotifyPlaylistsActionClearStorageStateSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionClearStorageStateFailure(ex.Message));
		}
	}
}
