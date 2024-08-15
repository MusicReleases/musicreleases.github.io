using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public class SpotifyArtistsController(IDispatcher dispatcher, IState<SpotifyArtistsState> stateSpotifyArtists) : ISpotifyArtistsController
{
	private readonly IDispatcher _dispatcher = dispatcher;
	private readonly IState<SpotifyArtistsState> _stateSpotifyArtists = stateSpotifyArtists;

	public void GetArtists(bool forceUpdate = false)
	{
		// action is running
		if (_stateSpotifyArtists.Value.LoadingAny())
		{
			return;
		}

		_dispatcher.Dispatch(new SpotifyArtistsActionGet(forceUpdate));
	}
}
