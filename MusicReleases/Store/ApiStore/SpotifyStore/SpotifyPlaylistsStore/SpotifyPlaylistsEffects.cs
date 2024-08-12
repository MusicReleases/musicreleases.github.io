using Blazored.LocalStorage;
using Fluxor;
using JakubKastner.SpotifyApi.Controllers;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

public class SpotifyPlaylistsEffects(ISpotifyControllerPlaylist spotifyControllerPlaylist, ILocalStorageService localStorageService)
{
	private readonly ISpotifyControllerPlaylist _spotifyControllerPlaylist = spotifyControllerPlaylist;
	private readonly ILocalStorageService _localStorageService = localStorageService;

	private const string _localStorageName = "spotify_playlists";


	[EffectMethod(typeof(SpotifyPlaylistsActionLoad))]
	public async Task LoadPlaylists(IDispatcher dispatcher)
	{
		try
		{
			var playlists = await _spotifyControllerPlaylist.GetUserPlaylists(true);
			dispatcher.Dispatch(new SpotifyPlaylistsActionSet(playlists));
			dispatcher.Dispatch(new SpotifyPlaylistsActionLoadSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionLoadFailure(ex.Message));
		}
	}

	// local storage:

	// PersistState
	[EffectMethod]
	public async Task SetStorage(SpotifyPlaylistsActionStorageSet action, IDispatcher dispatcher)
	{
		try
		{
			// set item
			await _localStorageService.SetItemAsync(_localStorageName, action.PlaylistsState);

			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageSetSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageSetFailure(ex.Message));
		}
	}

	[EffectMethod(typeof(SpotifyPlaylistsActionStorageGet))]
	public async Task LoadStorage(IDispatcher dispatcher)
	{
		try
		{
			// get item
			var playlistsState = await _localStorageService.GetItemAsync<SpotifyPlaylistsState>(_localStorageName);

			if (playlistsState is not null)
			{
				dispatcher.Dispatch(new SpotifyPlaylistsActionStorageSet(playlistsState));
				dispatcher.Dispatch(new SpotifyPlaylistsActionStorageGetSuccess());
			}
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageGetFailure(ex.Message));
		}
	}

	[EffectMethod(typeof(SpotifyPlaylistsActionStorageClear))]
	public async Task ClearStorage(IDispatcher dispatcher)
	{
		try
		{
			// remove item
			await _localStorageService.RemoveItemAsync(_localStorageName);

			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageSet(new()
			{
				Initialized = false,
				Loading = false,
				List = new(),
			}));
			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageClearSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistsActionStorageClearFailure(ex.Message));
		}
	}
}
