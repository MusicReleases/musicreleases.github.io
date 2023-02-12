using Fluxor;
using JakubKastner.MusicReleases.Store.Api.Spotify.Playlists;
using JakubKastner.MusicReleases.Store.Releases;

namespace JakubKastner.MusicReleases.Store.Loader;

public class LoaderEffects
{
	private readonly IState<SpotifyPlaylistsState> _spotifyPlaylistsState;

	public LoaderEffects(IState<SpotifyPlaylistsState> spotifyPlaylistsState)
	{
		_spotifyPlaylistsState = spotifyPlaylistsState;
	}

	[EffectMethod(typeof(SpotifyPlaylistsActionLoad))]
	public async Task LoadOnSpotifyPlaylistLoading(IDispatcher dispatcher)
	{
		await StartLoading(dispatcher);
	}

	[EffectMethod(typeof(SpotifyPlaylistsActionSet))]
	public async Task LoadOnSpotifyPlaylistLoaded(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}

	private async Task StartLoading(IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		if (_spotifyPlaylistsState.Value.Loading)
		{
			dispatcher.Dispatch(new LoaderAction(true));
		}
	}

	private async Task StopLoading(IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		if (!_spotifyPlaylistsState.Value.Loading)
		{
			dispatcher.Dispatch(new LoaderAction(false));
		}
	}
}
