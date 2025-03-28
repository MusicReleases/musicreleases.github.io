using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistStore;
using JakubKastner.SpotifyApi.Services;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;

public class SpotifyPlaylistTracksEffect(ISpotifyPlaylistService spotifyPlaylistService)
{
	private readonly ISpotifyPlaylistService _spotifyPlaylistService = spotifyPlaylistService;

	// GET
	[EffectMethod]
	public async Task Get(SpotifyPlaylistTrackActionGet action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		if (action.Playlists is null)
		{
			// provided playlists
			dispatcher.Dispatch(new SpotifyPlaylistActionGetStorage(action.ForceUpdate) { CompletionSource = action.CompletionSource });
			return;
		}
		dispatcher.Dispatch(new SpotifyPlaylistTrackActionGetApi(action.Playlists, action.ForceUpdate) { CompletionSource = action.CompletionSource });
	}
	[EffectMethod]
	public async Task GetSuccess(SpotifyPlaylistTrackActionGetSuccess action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		action.CompletionSource.SetResult(true);
	}
	[EffectMethod]
	public async Task GetFailure(SpotifyPlaylistTrackActionGetFailure action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		//#if DEBUG
		Console.WriteLine(action.ErrorMessage);
		//#endif
		action.CompletionSource.SetResult(false);
	}


	[EffectMethod]
	public async Task GetApi(SpotifyPlaylistTrackActionGetApi action, IDispatcher dispatcher)
	{
		try
		{
			// get item from api
			var playlistsStorage = action.Playlists;
			var playlistsApi = await _spotifyPlaylistService.GetPlaylistsTracks(playlistsStorage, action.ForceUpdate);
			dispatcher.Dispatch(new SpotifyPlaylistTrackActionGetApiSuccess());

			dispatcher.Dispatch(new SpotifyPlaylistActionSet(playlistsApi));
			dispatcher.Dispatch(new SpotifyPlaylistActionSetStorage(playlistsApi));
			dispatcher.Dispatch(new SpotifyPlaylistTrackActionGetSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyPlaylistTrackActionGetApiFailure());
			dispatcher.Dispatch(new SpotifyPlaylistTrackActionGetFailure(ex.Message));
		}
	}
}
