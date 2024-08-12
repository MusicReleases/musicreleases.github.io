using Blazored.LocalStorage;
using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.MusicReleases.Store.StorageStore.StorageActions;

namespace JakubKastner.MusicReleases.Store.StorageStore;

public class StorageEffects(IState<SpotifyPlaylistsState> spotifyPlaylistsState, IState<SpotifyArtistsState> spotifyArtistsState, IState<SpotifyReleasesState> spotifyReleasesState, ILocalStorageService localStorageService)
{
	private readonly IState<SpotifyPlaylistsState> _spotifyPlaylistsState = spotifyPlaylistsState;
	/*private readonly IState<SpotifyArtistsState> _spotifyArtistsState = spotifyArtistsState;
	private readonly IState<SpotifyReleasesState> _spotifyReleasesState = spotifyReleasesState;*/

	private readonly ILocalStorageService _localStorageService = localStorageService;
	private const string _localStorageNamePlaylists = "Spotify_Playlists";

	// spotify playlists
	[EffectMethod]
	public async Task SaveOnSpotifyPlalistsSet(SpotifyPlaylistsActionSet action, IDispatcher dispatcher)
	{
		await SaveToStorage(dispatcher, action.Playlists, _localStorageNamePlaylists);
	}

	private async Task SaveToStorage<T>(IDispatcher dispatcher, SpotifyUserList<T> spotifyUserList, string localStorageName) where T : SpotifyIdObject
	{
		try
		{
			// set item
			await _localStorageService.SetItemAsync(localStorageName, spotifyUserList);

			dispatcher.Dispatch(new StorageActionsSetSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new StorageActionsSetFailure(ex.Message));
		}
	}
}
