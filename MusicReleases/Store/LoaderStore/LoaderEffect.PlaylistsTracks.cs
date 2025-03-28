using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;

namespace JakubKastner.MusicReleases.Store.LoaderStore;

public partial class LoaderEffect
{
	// spotify PlaylistsTracks
	[EffectMethod(typeof(SpotifyPlaylistTrackActionGet))]
	public async Task LoadOnSpotifyPlaylistTracksLoading(IDispatcher dispatcher)
	{
		await SpotifyPlaylistLoading(dispatcher);
	}
	private async Task SpotifyPlaylistTracksLoading(IDispatcher dispatcher)
	{
		if (_spotifyPlaylistTrackState.Value.LoadingAny())
		{
			await StartLoading(dispatcher);
		}
	}


	// -> api
	[EffectMethod(typeof(SpotifyPlaylistTrackActionGetApi))]
	public async Task LoadOnSpotifyPlaylistsTracksGetApi(IDispatcher dispatcher)
	{
		await SpotifyPlaylistTracksLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistTrackActionGetApiSuccess))]
	public async Task LoadOnSpotifyPlaylistsTracksGetApiSuccess(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistTrackActionGetApiFailure))]
	public async Task LoadOnSpotifyPlaylistsTracksGetApiFailure(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
}
