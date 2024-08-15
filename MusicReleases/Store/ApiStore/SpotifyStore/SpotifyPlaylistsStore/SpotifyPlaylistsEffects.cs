using Blazored.LocalStorage;
using Fluxor;
using JakubKastner.SpotifyApi.Controllers;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

public class SpotifyPlaylistsEffects(ISpotifyControllerPlaylist spotifyControllerPlaylist, ILocalStorageService localStorageService)
{
	private const ServiceType serviceType = ServiceType.Spotify;

	private readonly string _localStorageName = GetLocalStorageKey(serviceType, LocalStorageKey.UserPlaylists);
	private readonly string _localStorageStateName = GetLocalStorageKey(serviceType, LocalStorageKey.UserPlaylistsState);

	private readonly ISpotifyControllerPlaylist _spotifyControllerPlaylist = spotifyControllerPlaylist;
	private readonly ILocalStorageService _localStorageService = localStorageService;

	// LOAD
	[EffectMethod]
	public async Task Load(SpotifyPlaylistsActionGet action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		try
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionGetStorage(action.ForceUpdate));
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionGetFailure(ex.Message));
		}
	}

	[EffectMethod]
	public async Task LoadStorage(SpotifyPlaylistsActionGetStorage action, IDispatcher dispatcher)
	{
		try
		{
			// get item
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
	public async Task LoadApi(SpotifyPlaylistsActionGetApi action, IDispatcher dispatcher)
	{
		try
		{
			var playlistsStorage = action.Playlists;
			var playlists = await _spotifyControllerPlaylist.GetUserPlaylists(true, playlistsStorage, action.ForceUpdate);

			dispatcher.Dispatch(new SpotifyPlaylistsActionSet(playlists));

			dispatcher.Dispatch(new SpotifyPlaylistsActionGetApiSuccess());

			dispatcher.Dispatch(new SpotifyPlaylistsActionGetSuccess());


			dispatcher.Dispatch(new SpotifyPlaylistsActionSetStorage(playlists));
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionGetApiFailure(ex.Message));
		}
	}

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

	// local storage:

	// TODO PersistState
	[EffectMethod]
	public async Task SetStorageState(SpotifyPlaylistsActionStorageStateSet action, IDispatcher dispatcher)
	{
		try
		{
			// set item
			await _localStorageService.SetItemAsync(_localStorageStateName, action.PlaylistsState);

			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageSetStateSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageSetStateFailure(ex.Message));
		}
	}


	[EffectMethod(typeof(SpotifyPlaylistsActionStorageGetState))]
	public async Task LoadStorageState(IDispatcher dispatcher)
	{
		try
		{
			// get item
			var playlistsState = await _localStorageService.GetItemAsync<SpotifyPlaylistsState>(_localStorageStateName);

			if (playlistsState is not null)
			{
				dispatcher.Dispatch(new SpotifyPlaylistsActionStorageStateSet(playlistsState));
				dispatcher.Dispatch(new SpotifyPlaylistsActionStorageGetStateSuccess());
			}
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageGetStateFailure(ex.Message));
		}
	}

	[EffectMethod(typeof(SpotifyPlaylistsActionStorageStateClear))]
	public async Task ClearStorageState(IDispatcher dispatcher)
	{
		try
		{
			// remove item
			await _localStorageService.RemoveItemAsync(_localStorageStateName);

			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageStateSet(new()
			{
				Initialized = false,
				LoadingStorage = false,
				LoadingApi = false,
				List = new(),
			}));
			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageClearStateSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageClearStateFailure(ex.Message));
		}
	}
}
