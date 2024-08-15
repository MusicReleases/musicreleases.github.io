using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

namespace JakubKastner.MusicReleases.Store.LoaderStore;

public class LoaderEffects(IState<SpotifyPlaylistsState> spotifyPlaylistsState, IState<SpotifyArtistsState> spotifyArtistsState, IState<SpotifyReleasesState> spotifyReleasesState)
{
	private readonly IState<SpotifyPlaylistsState> _spotifyPlaylistsState = spotifyPlaylistsState;
	private readonly IState<SpotifyArtistsState> _spotifyArtistsState = spotifyArtistsState;
	private readonly IState<SpotifyReleasesState> _spotifyReleasesState = spotifyReleasesState;

	// spotify playlists
	[EffectMethod(typeof(SpotifyPlaylistsActionGet))]
	public async Task LoadOnSpotifyPlaylistLoading(IDispatcher dispatcher)
	{
		await SpotifyPlaylistLoading(dispatcher);
	}
	private async Task SpotifyPlaylistLoading(IDispatcher dispatcher)
	{
		if (_spotifyPlaylistsState.Value.LoadingAny())
		{
			await StartLoading(dispatcher);
		}
	}


	// -> set
	[EffectMethod(typeof(SpotifyPlaylistsActionSet))]
	public async Task LoadOnSpotifyPlaylistSet(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}

	// -> storage
	[EffectMethod(typeof(SpotifyPlaylistsActionGetStorage))]
	public async Task LoadOnSpotifyPlaylistGetStorage(IDispatcher dispatcher)
	{
		await SpotifyPlaylistLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistsActionGetStorageSuccess))]
	public async Task LoadOnSpotifyPlaylistGetStorageSuccess(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistsActionGetStorageFailure))]
	public async Task LoadOnSpotifyPlaylistGetStorageFailure(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}

	// -> api
	[EffectMethod(typeof(SpotifyPlaylistsActionGetApi))]
	public async Task LoadOnSpotifyPlaylistGetApi(IDispatcher dispatcher)
	{
		await SpotifyPlaylistLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistsActionGetApiSuccess))]
	public async Task LoadOnSpotifyPlaylistGetApiSuccess(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistsActionGetApiFailure))]
	public async Task LoadOnSpotifyPlaylistGetApiFailure(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}


	// spotify artists
	[EffectMethod(typeof(SpotifyArtistsActionLoad))]
	public async Task LoadOnSpotifyArtistLoading(IDispatcher dispatcher)
	{
		if (_spotifyArtistsState.Value.Loading)
		{
			await StartLoading(dispatcher);
		}
	}
	[EffectMethod(typeof(SpotifyArtistsActionSet))]
	public async Task LoadOnSpotifyArtistLoaded(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}

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

		if (!_spotifyPlaylistsState.Value.LoadingAny() && !_spotifyArtistsState.Value.Loading && !_spotifyReleasesState.Value.Loading)
		{
			dispatcher.Dispatch(new LoaderAction(false));
		}
	}
}
