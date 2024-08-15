using Blazored.LocalStorage;
using Fluxor;
using JakubKastner.SpotifyApi.Controllers;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;

public class SpotifyArtistsEffects(ISpotifyControllerArtist spotifyControllerArtist, ILocalStorageService localStorageService, IState<SpotifyArtistsState> artistsState)
{
	private const ServiceType serviceType = ServiceType.Spotify;

	private readonly string _localStorageName = GetLocalStorageKey(serviceType, LocalStorageKey.UserArtists);
	private readonly string _localStorageStateName = GetLocalStorageKey(serviceType, LocalStorageKey.UserArtistsState);

	private readonly ISpotifyControllerArtist _spotifyControllerArtist = spotifyControllerArtist;
	private readonly ILocalStorageService _localStorageService = localStorageService;


	// GET
	[EffectMethod]
	public async Task Get(SpotifyArtistsActionGet action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		dispatcher.Dispatch(new SpotifyArtistsActionGetStorage(action.ForceUpdate));
	}

	[EffectMethod]
	public async Task GetStorage(SpotifyArtistsActionGetStorage action, IDispatcher dispatcher)
	{
		try
		{
			// get item from storage
			var artists = await _localStorageService.GetItemAsync<SpotifyUserList<SpotifyArtist>>(_localStorageName);

			if (artists is not null)
			{
				dispatcher.Dispatch(new SpotifyArtistsActionSet(artists));
			}
			dispatcher.Dispatch(new SpotifyArtistsActionGetStorageSuccess());
			dispatcher.Dispatch(new SpotifyArtistsActionGetApi(artists, action.ForceUpdate));
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyArtistsActionGetStorageFailure(ex.Message));
		}
	}

	[EffectMethod]
	public async Task GetApi(SpotifyArtistsActionGetApi action, IDispatcher dispatcher)
	{
		try
		{
			// get item from api
			var playlistsStorage = action.Artists;
			var playlists = await _spotifyControllerArtist.GetUserFollowedArtists(playlistsStorage, action.ForceUpdate);
			dispatcher.Dispatch(new SpotifyArtistsActionGetApiSuccess());

			dispatcher.Dispatch(new SpotifyArtistsActionSet(playlists));
			dispatcher.Dispatch(new SpotifyArtistsActionSetStorage(playlists));
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyArtistsActionGetApiFailure(ex.Message));
		}
	}

	// SET
	[EffectMethod]
	public async Task SetStorage(SpotifyArtistsActionSetStorage action, IDispatcher dispatcher)
	{
		try
		{
			// set item
			await _localStorageService.SetItemAsync(_localStorageName, action.Artists);

			dispatcher.Dispatch(new SpotifyArtistsActionSetStorageSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyArtistsActionSetStorageFailure(ex.Message));
		}
	}


	// TODO PERSIST STATE (copy from playlists)
}
