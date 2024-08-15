using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

namespace JakubKastner.MusicReleases.Store.LoaderStore;

public partial class LoaderEffects
{
	// spotify playlists
	[EffectMethod(typeof(SpotifyPlaylistsActionGet))]
	public async Task LoadOnSpotifyPlaylistLoading(IDispatcher dispatcher)
	{
		await SpotifyPlaylistLoading(dispatcher);
	}
	private async Task SpotifyPlaylistLoading(IDispatcher dispatcher)
	{
		if (_spotifyPlaylistsState.Value.LoadingAny())
		{
			await StartLoading(dispatcher);
		}
	}

	// -> set
	[EffectMethod(typeof(SpotifyPlaylistsActionSet))]
	public async Task LoadOnSpotifyPlaylistSet(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}

	// -> storage
	[EffectMethod(typeof(SpotifyPlaylistsActionGetStorage))]
	public async Task LoadOnSpotifyPlaylistGetStorage(IDispatcher dispatcher)
	{
		await SpotifyPlaylistLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistsActionGetStorageSuccess))]
	public async Task LoadOnSpotifyPlaylistGetStorageSuccess(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistsActionGetStorageFailure))]
	public async Task LoadOnSpotifyPlaylistGetStorageFailure(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}

	// -> api
	[EffectMethod(typeof(SpotifyPlaylistsActionGetApi))]
	public async Task LoadOnSpotifyPlaylistGetApi(IDispatcher dispatcher)
	{
		await SpotifyPlaylistLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistsActionGetApiSuccess))]
	public async Task LoadOnSpotifyPlaylistGetApiSuccess(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
	[EffectMethod(typeof(SpotifyPlaylistsActionGetApiFailure))]
	public async Task LoadOnSpotifyPlaylistGetApiFailure(IDispatcher dispatcher)
	{
		await StopLoading(dispatcher);
	}
}
