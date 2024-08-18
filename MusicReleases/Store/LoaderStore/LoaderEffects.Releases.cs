using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

namespace JakubKastner.MusicReleases.Store.LoaderStore;

public partial class LoaderEffects
{
	// spotify PlaylistsTracks
	[EffectMethod(typeof(SpotifyReleasesActionGet))]
	public async Task LoadOnSpotifyReleasesLoading(IDispatcher dispatcher)
	{
		await SpotifyPlaylistLoading(dispatcher);
	}
	private async Task SpotifyReleasesLoading(IDispatcher dispatcher)
	{
		if (_spotifyReleasesState.Value.LoadingAny())
		{
			await StartLoading(dispatcher);
		}
	}


	// -> api
	[EffectMethod(typeof(SpotifyReleasesActionGetApi))]
	public async Task LoadOnSpotifyReleasesGetApi(IDispatcher dispatcher)
	{
		await SpotifyReleasesLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyReleasesActionGetApiSuccess))]
	public async Task LoadOnSpotifyReleasesGetApiSuccess(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyReleasesActionGetApiFailure))]
	public async Task LoadOnSpotifyReleasesGetApiFailure(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
}
