using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleaseStore;

namespace JakubKastner.MusicReleases.Store.LoaderStore;

public partial class LoaderEffect(IState<SpotifyPlaylistState> spotifyPlaylistState, IState<SpotifyPlaylistTrackState> spotifyPlaylistTrackState, IState<SpotifyArtistState> spotifyArtistState, IState<SpotifyReleaseState> spotifyReleaseState)
{
	private readonly IState<SpotifyPlaylistState> _spotifyPlaylistState = spotifyPlaylistState;
	private readonly IState<SpotifyPlaylistTrackState> _spotifyPlaylistTrackState = spotifyPlaylistTrackState;
	private readonly IState<SpotifyArtistState> _spotifyArtistState = spotifyArtistState;
	private readonly IState<SpotifyReleaseState> _spotifyReleaseState = spotifyReleaseState;

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

		if (!_spotifyPlaylistState.Value.LoadingAny() && !_spotifyPlaylistTrackState.Value.LoadingAny() && !_spotifyArtistState.Value.LoadingAny() && !_spotifyReleaseState.Value.LoadingAny())
		{
			dispatcher.Dispatch(new LoaderAction(false));
		}
	}
}
