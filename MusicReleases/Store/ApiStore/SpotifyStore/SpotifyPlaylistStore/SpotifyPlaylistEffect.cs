using Blazored.LocalStorage;
using Fluxor;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.Services;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistStore;

public class SpotifyPlaylistEffect(ISpotifyPlaylistService spotifyPlaylistService, ILocalStorageService localStorageService)
{
	private const ServiceType serviceType = ServiceType.Spotify;

	private readonly string _localStorageName = GetLocalStorageKey(serviceType, LocalStorageKey.UserPlaylists);

	private readonly ISpotifyPlaylistService _spotifyPlaylistService = spotifyPlaylistService;
	private readonly ILocalStorageService _localStorageService = localStorageService;

	// GET
	[EffectMethod]
	public async Task Get(SpotifyPlaylistActionGet action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		dispatcher.Dispatch(new SpotifyPlaylistActionGetStorage(action.ForceUpdate) { CompletionSource = action.CompletionSource });
	}

	[EffectMethod]
	public async Task GetSuccess(SpotifyPlaylistActionGetSuccess action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		action.CompletionSource.SetResult(true);
	}
	[EffectMethod]
	public async Task GetFailure(SpotifyPlaylistActionGetFailure action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		//#if DEBUG
		Console.WriteLine(action.ErrorMessage);
		//#endif
		action.CompletionSource.SetResult(false);
	}

	[EffectMethod]
	public async Task GetStorage(SpotifyPlaylistActionGetStorage action, IDispatcher dispatcher)
	{
		try
		{
			// get item from storage
			var playlists = await _localStorageService.GetItemAsync<SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>>(_localStorageName);

			if (playlists is not null)
			{
				dispatcher.Dispatch(new SpotifyPlaylistActionSet(playlists));
			}
			dispatcher.Dispatch(new SpotifyPlaylistActionGetStorageSuccess());
			dispatcher.Dispatch(new SpotifyPlaylistActionGetApi(playlists, action.ForceUpdate) { CompletionSource = action.CompletionSource });
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistActionGetStorageFailure());
			dispatcher.Dispatch(new SpotifyPlaylistActionGetFailure(ex.Message) { CompletionSource = action.CompletionSource });
		}
	}

	[EffectMethod]
	public async Task GetApi(SpotifyPlaylistActionGetApi action, IDispatcher dispatcher)
	{
		try
		{
			// get item from api
			var playlistsStorage = action.Playlists;
			var playlists = await _spotifyPlaylistService.GetUserPlaylists(true, playlistsStorage, action.ForceUpdate);
			dispatcher.Dispatch(new SpotifyPlaylistActionGetApiSuccess());

			dispatcher.Dispatch(new SpotifyPlaylistActionSet(playlists));
			dispatcher.Dispatch(new SpotifyPlaylistActionSetStorage(playlists));
			dispatcher.Dispatch(new SpotifyPlaylistActionGetSuccess() { CompletionSource = action.CompletionSource });
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistActionGetApiFailure());
			dispatcher.Dispatch(new SpotifyPlaylistActionGetFailure(ex.Message) { CompletionSource = action.CompletionSource });
		}
	}


	// SET
	[EffectMethod]
	public async Task SetStorage(SpotifyPlaylistActionSetStorage action, IDispatcher dispatcher)
	{
		try
		{
			// set item
			await _localStorageService.SetItemAsync(_localStorageName, action.Playlists);

			dispatcher.Dispatch(new SpotifyPlaylistActionSetStorageSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistActionSetStorageFailure(ex.Message));
		}
	}
	[EffectMethod]
	public async Task SetStorageFailure(SpotifyPlaylistActionSetStorageFailure action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		//#if DEBUG
		Console.WriteLine(action.ErrorMessage);
		//#endif
	}


	// TODO PERSIST STATE

	// TODO persist state when failed ??
	/*[EffectMethod(typeof(SpotifyPlaylistsActionGetApiFailure))]
	public async Task GetApiFailure(IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		dispatcher.Dispatch(new SpotifyPlaylistsActionSetStorageState(_playlistsState.Value));
	}*/

	/*[EffectMethod(typeof(SpotifyPlaylistsActionGetStorageState))]
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
			dispatcher.Dispatch(new SpotifyPlaylistsActionGetStorageStateFailure());
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
			dispatcher.Dispatch(new SpotifyPlaylistsActionSetStorageStateFailure());
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
			dispatcher.Dispatch(new SpotifyPlaylistsActionClearStorageStateFailure());
		}
	}*/
}
