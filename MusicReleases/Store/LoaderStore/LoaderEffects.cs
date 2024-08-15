using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

namespace JakubKastner.MusicReleases.Store.LoaderStore;

public partial class LoaderEffects(IState<SpotifyPlaylistsState> spotifyPlaylistsState, IState<SpotifyArtistsState> spotifyArtistsState, IState<SpotifyReleasesState> spotifyReleasesState)
{
	private readonly IState<SpotifyPlaylistsState> _spotifyPlaylistsState = spotifyPlaylistsState;
	private readonly IState<SpotifyArtistsState> _spotifyArtistsState = spotifyArtistsState;
	private readonly IState<SpotifyReleasesState> _spotifyReleasesState = spotifyReleasesState;


	// spotify releases
	[EffectMethod(typeof(SpotifyReleasesActionLoad))]
	public async Task LoadOnSpotifyReleasesLoading(IDispatcher dispatcher)
	{
		if (_spotifyReleasesState.Value.Loading)
		{
			await StartLoading(dispatcher);
		}
	}
	[EffectMethod(typeof(SpotifyReleasesActionSet))]
	public async Task LoadOnSpotifyReleasesLoaded(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}



	private async Task StartLoading(IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		dispatcher.Dispatch(new LoaderAction(true));
	}

	private async Task StopLoading(IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		if (!_spotifyPlaylistsState.Value.LoadingAny() && !_spotifyArtistsState.Value.LoadingAny() && !_spotifyReleasesState.Value.Loading)
		{
			dispatcher.Dispatch(new LoaderAction(false));
		}
	}
}
