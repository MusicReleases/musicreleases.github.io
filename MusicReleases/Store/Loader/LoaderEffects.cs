using Fluxor;
using JakubKastner.MusicReleases.Store.Api.Spotify.Artists;
using JakubKastner.MusicReleases.Store.Api.Spotify.Playlists;
using JakubKastner.MusicReleases.Store.Api.Spotify.Releases;
using JakubKastner.MusicReleases.Store.Releases;

namespace JakubKastner.MusicReleases.Store.Loader;

public class LoaderEffects
{
	private readonly IState<SpotifyPlaylistsState> _spotifyPlaylistsState;
	private readonly IState<SpotifyArtistsState> _spotifyArtistsState;
	private readonly IState<SpotifyReleasesState> _spotifyReleasesState;

	public LoaderEffects(IState<SpotifyPlaylistsState> spotifyPlaylistsState, IState<SpotifyArtistsState> spotifyArtistsState, IState<SpotifyReleasesState> spotifyReleasesState)
	{
		_spotifyPlaylistsState = spotifyPlaylistsState;
		_spotifyArtistsState = spotifyArtistsState;
		_spotifyReleasesState = spotifyReleasesState;
	}

	// spotify playlists
	[EffectMethod(typeof(SpotifyPlaylistsActionLoad))]
	public async Task LoadOnSpotifyPlaylistLoading(IDispatcher dispatcher)
	{
		if (_spotifyPlaylistsState.Value.Loading)
		{
			await StartLoading(dispatcher);
		}
	}
	[EffectMethod(typeof(SpotifyPlaylistsActionSet))]
	public async Task LoadOnSpotifyPlaylistLoaded(IDispatcher dispatcher)
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

		if (!_spotifyPlaylistsState.Value.Loading && !_spotifyArtistsState.Value.Loading && !_spotifyReleasesState.Value.Loading)
		{
			dispatcher.Dispatch(new LoaderAction(false));
		}
	}
}
