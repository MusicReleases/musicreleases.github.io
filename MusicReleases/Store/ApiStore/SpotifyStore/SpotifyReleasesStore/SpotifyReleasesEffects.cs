using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;
using JakubKastner.SpotifyApi.Controllers;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasestore;

public class SpotifyReleasesEffects(ISpotifyControllerRelease spotifyControllerRelease)
{
	private readonly ISpotifyControllerRelease _spotifyControllerRelease = spotifyControllerRelease;

	// GET
	[EffectMethod]
	public async Task Get(SpotifyReleasesActionGet action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		if (action.Artists is null)
		{
			// provided artists
			var getStorageAction = new SpotifyArtistsActionGetStorage(action.ForceUpdate)
			{
				CompletionSource = action.CompletionSource,
			};
			dispatcher.Dispatch(getStorageAction);
			return;
		}

		var getApiAction = new SpotifyReleasesActionGetApi(action.ReleaseType, action.Artists, action.ForceUpdate)
		{
			CompletionSource = action.CompletionSource,
		};
		dispatcher.Dispatch(getApiAction);
	}
	[EffectMethod]
	public async Task GetSuccess(SpotifyReleasesActionGetSuccess action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		action.CompletionSource.SetResult(true);
	}
	[EffectMethod]
	public async Task GetFailure(SpotifyReleasesActionGetFailure action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		//#if DEBUG
		Console.WriteLine(action.ErrorMessage);
		//#endif
		action.CompletionSource.SetResult(false);
	}
	[EffectMethod]
	public async Task GetApiFailure(SpotifyReleasesActionGetApiFailure action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		//#if DEBUG
		Console.WriteLine(action.ErrorMessage);
		//#endif
		action.CompletionSource.SetResult(false);
	}

	[EffectMethod]
	public async Task GetApi(SpotifyReleasesActionGetApi action, IDispatcher dispatcher)
	{
		try
		{
			// get item from api
			var artistsStorage = action.Artists;
			var artistsApi = await _spotifyControllerRelease.GetReleasesFromArtist(action.ReleaseType, artistsStorage, action.ForceUpdate);
			dispatcher.Dispatch(new SpotifyReleasesActionGetApiSuccess());

			dispatcher.Dispatch(new SpotifyArtistsActionSet(artistsApi));
			dispatcher.Dispatch(new SpotifyArtistsActionSetStorage(artistsApi));
			dispatcher.Dispatch(new SpotifyReleasesActionGetSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyReleasesActionGetApiFailure(ex.Message));
			dispatcher.Dispatch(new SpotifyReleasesActionGetFailure(ex.Message));
		}
	}


	// TODO PERSIST STATE (copy from playlists)
}
