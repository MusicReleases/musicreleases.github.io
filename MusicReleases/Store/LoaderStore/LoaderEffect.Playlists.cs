using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistStore;

namespace JakubKastner.MusicReleases.Store.LoaderStore;

public partial class LoaderEffect
{
	// spotify playlists
	[EffectMethod(typeof(SpotifyPlaylistActionGet))]
	public async Task LoadOnSpotifyPlaylistLoading(IDispatcher dispatcher)
	{
		await SpotifyPlaylistLoading(dispatcher);
	}
	private async Task SpotifyPlaylistLoading(IDispatcher dispatcher)
	{
		if (_spotifyPlaylistState.Value.LoadingAny())
		{
			await StartLoading(dispatcher);
		}
	}

	// -> set
	[EffectMethod(typeof(SpotifyPlaylistActionSet))]
	public async Task LoadOnSpotifyPlaylistSet(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}

	// -> storage
	[EffectMethod(typeof(SpotifyPlaylistActionGetStorage))]
	public async Task LoadOnSpotifyPlaylistGetStorage(IDispatcher dispatcher)
	{
		await SpotifyPlaylistLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistActionGetStorageSuccess))]
	public async Task LoadOnSpotifyPlaylistGetStorageSuccess(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistActionGetStorageFailure))]
	public async Task LoadOnSpotifyPlaylistGetStorageFailure(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}

	// -> api
	[EffectMethod(typeof(SpotifyPlaylistActionGetApi))]
	public async Task LoadOnSpotifyPlaylistGetApi(IDispatcher dispatcher)
	{
		await SpotifyPlaylistLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistActionGetApiSuccess))]
	public async Task LoadOnSpotifyPlaylistGetApiSuccess(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistActionGetApiFailure))]
	public async Task LoadOnSpotifyPlaylistGetApiFailure(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
}
