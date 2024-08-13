using Blazored.LocalStorage;
using Fluxor;
using JakubKastner.SpotifyApi.Controllers;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

public class SpotifyPlaylistsEffects(ISpotifyControllerPlaylist spotifyControllerPlaylist, ILocalStorageService localStorageService)
{
	private readonly ISpotifyControllerPlaylist _spotifyControllerPlaylist = spotifyControllerPlaylist;
	private readonly ILocalStorageService _localStorageService = localStorageService;

	private const string _localStorageName = "Spotify_Playlists";
	private const string _localStorageNameState = _localStorageName + "_State";


	// LOAD

	[EffectMethod(typeof(SpotifyPlaylistsActionLoad))]
	public async Task Load(IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		try
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageGet());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionLoadFailure(ex.Message));
		}
	}

	[EffectMethod(typeof(SpotifyPlaylistsActionStorageGet))]
	public async Task LoadStorage(IDispatcher dispatcher)
	{
		try
		{
			// get item
			var playlists = await _localStorageService.GetItemAsync<SpotifyUserList<SpotifyPlaylist>>(_localStorageName);

			if (playlists is not null)
			{
				dispatcher.Dispatch(new SpotifyPlaylistsActionSet(playlists));
			}
			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageGetSuccess());
			dispatcher.Dispatch(new SpotifyPlaylistsActionApiLoad(playlists));
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageGetFailure(ex.Message));
		}
	}


	[EffectMethod]
	public async Task LoadApi(SpotifyPlaylistsActionApiLoad action, IDispatcher dispatcher)
	{
		try
		{
			var playlistsStorage = action.Playlists;
			var playlists = await _spotifyControllerPlaylist.GetUserPlaylists(true, playlistsStorage);

			dispatcher.Dispatch(new SpotifyPlaylistsActionSet(playlists));

			dispatcher.Dispatch(new SpotifyPlaylistsActionApiLoadSuccess());

			dispatcher.Dispatch(new SpotifyPlaylistsActionLoadSuccess());


			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageSet(playlists));
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionApiLoadFailure(ex.Message));
		}
	}

	[EffectMethod]
	public async Task SetStorage(SpotifyPlaylistsActionStorageSet action, IDispatcher dispatcher)
	{
		try
		{
			// set item
			await _localStorageService.SetItemAsync(_localStorageName, action.Playlists);

			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageSetSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageSetFailure(ex.Message));
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
			await _localStorageService.SetItemAsync(_localStorageNameState, action.PlaylistsState);

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
			var playlistsState = await _localStorageService.GetItemAsync<SpotifyPlaylistsState>(_localStorageNameState);

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
			await _localStorageService.RemoveItemAsync(_localStorageNameState);

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
