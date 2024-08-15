using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;

namespace JakubKastner.MusicReleases.Store.LoaderStore;

public partial class LoaderEffects
{
	// spotify artists
	[EffectMethod(typeof(SpotifyArtistsActionGet))]
	public async Task LoadOnSpotifyArtistLoading(IDispatcher dispatcher)
	{
		await SpotifyArtistLoading(dispatcher);
	}
	private async Task SpotifyArtistLoading(IDispatcher dispatcher)
	{
		if (_spotifyArtistsState.Value.LoadingAny())
		{
			await StartLoading(dispatcher);
		}
	}

	// -> set
	[EffectMethod(typeof(SpotifyArtistsActionSet))]
	public async Task LoadOnSpotifyArtistSet(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}

	// -> storage
	[EffectMethod(typeof(SpotifyArtistsActionGetStorage))]
	public async Task LoadOnSpotifyArtistGetStorage(IDispatcher dispatcher)
	{
		await SpotifyArtistLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyArtistsActionGetStorageSuccess))]
	public async Task LoadOnSpotifyArtistGetStorageSuccess(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyArtistsActionGetStorageFailure))]
	public async Task LoadOnSpotifyArtistGetStorageFailure(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}

	// -> api
	[EffectMethod(typeof(SpotifyArtistsActionGetApi))]
	public async Task LoadOnSpotifyArtistGetApi(IDispatcher dispatcher)
	{
		await SpotifyArtistLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyArtistsActionGetApiSuccess))]
	public async Task LoadOnSpotifyArtistGetApiSuccess(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyArtistsActionGetApiFailure))]
	public async Task LoadOnSpotifyArtistGetApiFailure(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
}
