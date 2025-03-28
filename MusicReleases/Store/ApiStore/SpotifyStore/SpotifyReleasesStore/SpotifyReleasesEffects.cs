using Fluxor;
using JakubKastner.MusicReleases.Controllers.DatabaseControllers;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;
using JakubKastner.SpotifyApi.Controllers;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasestore;

public class SpotifyReleasesEffects(ISpotifyControllerRelease spotifyControllerRelease, IDatabaseSpotifyArtistReleaseController databaseController, ISpotifyControllerUser spotifyControllerUser)
{
	private readonly ISpotifyControllerRelease _spotifyControllerRelease = spotifyControllerRelease;
	private readonly IDatabaseSpotifyArtistReleaseController _databaseController = databaseController;
	private readonly ISpotifyControllerUser _spotifyControllerUser = spotifyControllerUser;

	// GET
	[EffectMethod]
	public async Task Get(SpotifyReleasesActionGet action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		if (action.Artists is null)
		{
			// provided artists
			var getArtistsStorageAction = new SpotifyArtistsActionGetStorage(action.ForceUpdate)
			{
				CompletionSource = action.CompletionSource,
			};
			dispatcher.Dispatch(getArtistsStorageAction);
			return;
		}

		var getStorageAction = new SpotifyReleasesActionGetStorage(action.ReleaseType, action.Artists, action.ForceUpdate)
		{
			CompletionSource = action.CompletionSource,
		};
		dispatcher.Dispatch(getStorageAction);
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
	public async Task GetStorage(SpotifyReleasesActionGetStorage action, IDispatcher dispatcher)
	{
		/*try
		{*/
		// clear new releases
		dispatcher.Dispatch(new SpotifyReleasesNewActionClear());

		// get item from storage
		var userId = _spotifyControllerUser.GetUserIdRequired();
		// TODO list null
		var releases = await _databaseController.Get(action.Artists.List, userId);

		if (releases is not null)
		{
			dispatcher.Dispatch(new SpotifyReleasesActionSet(releases));
		}
		dispatcher.Dispatch(new SpotifyReleasesActionGetStorageSuccess());
		dispatcher.Dispatch(new SpotifyReleasesActionGetApi(action.ReleaseType, action.Artists, releases, action.ForceUpdate) { CompletionSource = action.CompletionSource });
		/*}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyReleasesActionGetStorageFailure());
			dispatcher.Dispatch(new SpotifyReleasesActionGetFailure(ex.Message) { CompletionSource = action.CompletionSource });
		}*/
	}

	[EffectMethod]
	public async Task GetApi(SpotifyReleasesActionGetApi action, IDispatcher dispatcher)
	{
		try
		{
			// get item from api
			// TODO null
			var releasesApi = await _spotifyControllerRelease.GetReleases(action.ReleaseType, action.Artists.List, action.Releases, action.ForceUpdate);
			dispatcher.Dispatch(new SpotifyReleasesActionGetApiSuccess());

			dispatcher.Dispatch(new SpotifyReleasesActionSet(releasesApi));
			dispatcher.Dispatch(new SpotifyReleasesActionSetStorage(releasesApi));
			dispatcher.Dispatch(new SpotifyReleasesActionGetSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyReleasesActionGetApiFailure());
			dispatcher.Dispatch(new SpotifyReleasesActionGetFailure(ex.Message));
		}
	}
	// SET
	[EffectMethod]
	public async Task SetStorage(SpotifyReleasesActionSetStorage action, IDispatcher dispatcher)
	{
		try
		{
			// set item
			var userId = _spotifyControllerUser.GetUserIdRequired();
			await _databaseController.Save(userId, action.Releases);

			dispatcher.Dispatch(new SpotifyReleasesActionSetStorageSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyReleasesActionSetStorageFailure(ex.Message));
		}
	}
	[EffectMethod]
	public async Task SetStorageFailure(SpotifyReleasesActionSetStorageFailure action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		//#if DEBUG
		Console.WriteLine(action.ErrorMessage);
		//#endif
	}


	// TODO PERSIST STATE (copy from playlists)
}
