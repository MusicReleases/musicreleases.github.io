using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistStore;

namespace JakubKastner.MusicReleases.Store.LoaderStore;

public partial class LoaderEffect
{
	// spotify artists
	[EffectMethod(typeof(SpotifyArtistActionGet))]
	public async Task LoadOnSpotifyArtistLoading(IDispatcher dispatcher)
	{
		await SpotifyArtistLoading(dispatcher);
	}
	private async Task SpotifyArtistLoading(IDispatcher dispatcher)
	{
		if (_spotifyArtistState.Value.LoadingAny())
		{
			await StartLoading(dispatcher);
		}
	}

	// -> set
	[EffectMethod(typeof(SpotifyArtistActionSet))]
	public async Task LoadOnSpotifyArtistSet(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}

	// -> storage
	[EffectMethod(typeof(SpotifyArtistActionGetStorage))]
	public async Task LoadOnSpotifyArtistGetStorage(IDispatcher dispatcher)
	{
		await SpotifyArtistLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyArtistActionGetStorageSuccess))]
	public async Task LoadOnSpotifyArtistGetStorageSuccess(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyArtistActionGetStorageFailure))]
	public async Task LoadOnSpotifyArtistGetStorageFailure(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}

	// -> api
	[EffectMethod(typeof(SpotifyArtistActionGetApi))]
	public async Task LoadOnSpotifyArtistGetApi(IDispatcher dispatcher)
	{
		await SpotifyArtistLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyArtistActionGetApiSuccess))]
	public async Task LoadOnSpotifyArtistGetApiSuccess(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyArtistActionGetApiFailure))]
	public async Task LoadOnSpotifyArtistGetApiFailure(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
}
