using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleaseStore;

namespace JakubKastner.MusicReleases.Store.LoaderStore;

public partial class LoaderEffect
{
	// spotify PlaylistsTracks
	[EffectMethod(typeof(SpotifyReleaseActionGet))]
	public async Task LoadOnSpotifyReleasesLoading(IDispatcher dispatcher)
	{
		await SpotifyPlaylistLoading(dispatcher);
	}
	private async Task SpotifyReleasesLoading(IDispatcher dispatcher)
	{
		if (_spotifyReleaseState.Value.LoadingAny())
		{
			await StartLoading(dispatcher);
		}
	}


	// -> api
	[EffectMethod(typeof(SpotifyReleaseActionGetApi))]
	public async Task LoadOnSpotifyReleasesGetApi(IDispatcher dispatcher)
	{
		await SpotifyReleasesLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyReleaseActionGetApiSuccess))]
	public async Task LoadOnSpotifyReleasesGetApiSuccess(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyReleaseActionGetApiFailure))]
	public async Task LoadOnSpotifyReleasesGetApiFailure(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
}
