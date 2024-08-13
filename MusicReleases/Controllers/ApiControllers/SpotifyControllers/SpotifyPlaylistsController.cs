using Blazored.LocalStorage;
using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public class SpotifyPlaylistsController(IDispatcher dispatcher, IState<SpotifyPlaylistsState> stateSpotifyPlaylists, ILocalStorageService localStorageService) : ISpotifyPlaylistsController
{
	private readonly IDispatcher _dispatcher = dispatcher;
	private readonly IState<SpotifyPlaylistsState> _stateSpotifyPlaylists = stateSpotifyPlaylists;
	private readonly ILocalStorageService _localStorageService = localStorageService;

	//private ISet<SpotifyPlaylist>? _playlists => _stateSpotifyPlaylists.Value.Playlists;
	//private bool _loading => _stateSpotifyPlaylists.Value.Loading2;
	private bool _loading => _stateSpotifyPlaylists.Value.LoadingAny();
	private readonly string _storageName = GetLocalStorageKey(ServiceType.Spotify, LocalStorageKey.UserPlaylists);


	public void GetPlaylists()
	{
		// action is running
		if (_loading)
		{
			return;
		}

		if (!_stateSpotifyPlaylists.Value.Initialized)
		{
			_dispatcher.Dispatch(new SpotifyPlaylistsActionInitialized());
		}

		//GetPlaylistsFromStorage();
		GetPlaylistsFromApi();
		//SavePlaylistsToStorage();
	}

	private void GetPlaylistsFromStorage()
	{
		// previously saved playlists in browser storage
		//_dispatcher.Dispatch(new StorageActionsGet());
		//var playlists = await _localStorageService.GetItemAsync<SpotifyUserList<SpotifyPlaylist>>(_storageName);
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
	}

	private void SavePlaylistsToStorage()
	{

		// new loading from api
		if (_loading)
		{
			// action is running
			return;
		}

		// not initialized
		if (!_stateSpotifyPlaylists.Value.Initialized)
		{
			return;
		}

		_dispatcher.Dispatch(new SpotifyPlaylistsActionStorageStateSet(_stateSpotifyPlaylists.Value));
	}
}
