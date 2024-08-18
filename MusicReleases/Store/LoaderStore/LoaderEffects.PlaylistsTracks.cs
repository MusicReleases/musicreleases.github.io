using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;

namespace JakubKastner.MusicReleases.Store.LoaderStore;

public partial class LoaderEffects
{
	// spotify PlaylistsTracks
	[EffectMethod(typeof(SpotifyPlaylistsTracksActionGet))]
	public async Task LoadOnSpotifyPlaylistTracksLoading(IDispatcher dispatcher)
	{
		await SpotifyPlaylistLoading(dispatcher);
	}
	private async Task SpotifyPlaylistTracksLoading(IDispatcher dispatcher)
	{
		if (_spotifyPlaylistsTracksState.Value.LoadingAny())
		{
			await StartLoading(dispatcher);
		}
	}


	// -> api
	[EffectMethod(typeof(SpotifyPlaylistsTracksActionGetApi))]
	public async Task LoadOnSpotifyPlaylistsTracksGetApi(IDispatcher dispatcher)
	{
		await SpotifyPlaylistTracksLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistsTracksActionGetApiSuccess))]
	public async Task LoadOnSpotifyPlaylistsTracksGetApiSuccess(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistsTracksActionGetApiFailure))]
	public async Task LoadOnSpotifyPlaylistsTracksGetApiFailure(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
}
