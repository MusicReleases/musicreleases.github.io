using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public class SpotifyPlaylistsController(IDispatcher dispatcher, IState<SpotifyPlaylistsState> stateSpotifyPlaylists) : ISpotifyPlaylistsController
{
	private readonly IDispatcher _dispatcher = dispatcher;
	private readonly IState<SpotifyPlaylistsState> _stateSpotifyPlaylists = stateSpotifyPlaylists;

	public void GetPlaylists(bool forceUpdate = false)
	{
		// action is running
		if (_stateSpotifyPlaylists.Value.LoadingAny())
		{
			return;
		}

		_dispatcher.Dispatch(new SpotifyPlaylistsActionGet(forceUpdate));
	}
}
