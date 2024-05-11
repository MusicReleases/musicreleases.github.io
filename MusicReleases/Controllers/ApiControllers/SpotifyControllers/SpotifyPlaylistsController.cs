using Blazored.LocalStorage;
using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public class SpotifyPlaylistsController(IDispatcher dispatcher, IState<SpotifyPlaylistsState> stateSpotifyPlaylists, ILocalStorageService localStorageService) : ISpotifyPlaylistsController
{
	private readonly IDispatcher _dispatcher = dispatcher;
	private readonly IState<SpotifyPlaylistsState> _stateSpotifyPlaylists = stateSpotifyPlaylists;
	private readonly ILocalStorageService _localStorageService = localStorageService;

	//private ISet<SpotifyPlaylist>? _playlists => _stateSpotifyPlaylists.Value.Playlists;
	private bool _loading => _stateSpotifyPlaylists.Value.Loading;
	private readonly string _storageName = GetLocalStorageKey(ServiceType.Spotify, LocalStorageKey.UserPlaylists);


	public void GetPlaylists()
	{
		// action is running
		if (_loading)
		{
			return;
		}

		GetPlaylistsFromApi();
	}

	private async Task GetPlaylistsFromStorage()
	{
		// previously saved playlists in browser storage
		var playlists = await _localStorageService.GetItemAsync<SpotifyUserList<SpotifyPlaylist>>(_storageName);
	}

	private void GetPlaylistsFromStore()
	{
		// saved loading request from api
	}

	private void GetPlaylistsFromApi()
	{
		// new loading from api
		if (_loading)
		{
			// action is running
			return;
		}

		_dispatcher.Dispatch(new SpotifyPlaylistsActionLoad());

		if (!_stateSpotifyPlaylists.Value.Initialized)
		{
			_dispatcher.Dispatch(new SpotifyPlaylistsActionInitialized());
		}
	}
}
