using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;
using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public class SpotifyPlaylistsController(IDispatcher dispatcher, IState<SpotifyPlaylistsState> stateSpotifyPlaylists) : ISpotifyPlaylistsController
{
	private readonly IDispatcher _dispatcher = dispatcher;
	private readonly IState<SpotifyPlaylistsState> _stateSpotifyPlaylists = stateSpotifyPlaylists;

	private bool _loading => _stateSpotifyPlaylists.Value.LoadingAny();
	private readonly string _storageName = GetLocalStorageKey(ServiceType.Spotify, LocalStorageKey.UserPlaylists);


	public void GetPlaylists(bool forceUpdate = false)
	{
		// action is running
		if (_loading)
		{
			return;
		}

		_dispatcher.Dispatch(new SpotifyPlaylistsActionGet(forceUpdate));
	}
}
