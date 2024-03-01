using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public class SpotifyReleasesController(IDispatcher dispatcher, IState<SpotifyReleasesState> stateSpotifyReleases, IState<SpotifyArtistsState> stateSpotifyArtists) : ISpotifyReleasesController
{
	private readonly IDispatcher _dispatcher = dispatcher;
	private readonly IState<SpotifyReleasesState> _stateSpotifyReleases = stateSpotifyReleases;
	private readonly IState<SpotifyArtistsState> _stateSpotifyArtists = stateSpotifyArtists;

	public SortedSet<SpotifyRelease>? Releases => _stateSpotifyReleases.Value.Releases;
	public bool Loading => _stateSpotifyReleases.Value.Loading;

	public void LoadReleases(ReleaseType releaseType)
	{
		// TODO loading & loaded
		/*if (_stateSpotifyReleases.Value.Initialized == false)
		{*/
		_dispatcher.Dispatch(new SpotifyReleasesActionLoad(releaseType));
		_dispatcher.Dispatch(new SpotifyReleasesActionInitialized());
		// }
	}

	public void SaveReleasesToStorage()
	{
		_dispatcher.Dispatch(new SpotifyArtistsActionStorageSet(_stateSpotifyArtists.Value));
	}
}
