using Fluxor;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleaseStore;
using JakubKastner.SpotifyApi.Services;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasestore;

public class SpotifyReleaseEffect(ISpotifyReleaseService spotifyReleaseService, ISpotifyUserService spotifyUserService, IDbSpotifyArtistReleaseService dbSpotifyArtistReleaseService)
{
	private readonly ISpotifyReleaseService _spotifyReleaseService = spotifyReleaseService;
	private readonly ISpotifyUserService _spotifyUserService = spotifyUserService;
	private readonly IDbSpotifyArtistReleaseService _dbSpotifyArtistReleaseService = dbSpotifyArtistReleaseService;

	// GET
	[EffectMethod]
	public async Task Get(SpotifyReleaseActionGet action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		if (action.Artists is null)
		{
			// provided artists
			var getArtistsStorageAction = new SpotifyArtistActionGetStorage(action.ForceUpdate)
			{
				CompletionSource = action.CompletionSource,
			};
			dispatcher.Dispatch(getArtistsStorageAction);
			return;
		}

		var getStorageAction = new SpotifyReleaseActionGetStorage(action.ReleaseType, action.Artists, action.ForceUpdate)
		{
			CompletionSource = action.CompletionSource,
		};
		dispatcher.Dispatch(getStorageAction);
	}
	[EffectMethod]
	public async Task GetSuccess(SpotifyReleaseActionGetSuccess action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		action.CompletionSource.SetResult(true);
	}
	[EffectMethod]
	public async Task GetFailure(SpotifyReleaseActionGetFailure action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		//#if DEBUG
		Console.WriteLine(action.ErrorMessage);
		//#endif
		action.CompletionSource.SetResult(false);
	}

	[EffectMethod]
	public async Task GetStorage(SpotifyReleaseActionGetStorage action, IDispatcher dispatcher)
	{
		/*try
		{*/
		// clear new releases
		dispatcher.Dispatch(new SpotifyReleaseNewActionClear());

		// get item from storage
		var userId = _spotifyUserService.GetUserIdRequired();
		// TODO list null
		var releases = await _dbSpotifyArtistReleaseService.Get(action.Artists.List, userId);

		if (releases is not null)
		{
			dispatcher.Dispatch(new SpotifyReleaseActionSet(releases));
		}
		dispatcher.Dispatch(new SpotifyReleaseActionGetStorageSuccess());
		dispatcher.Dispatch(new SpotifyReleaseActionGetApi(action.ReleaseType, action.Artists, releases, action.ForceUpdate) { CompletionSource = action.CompletionSource });
		/*}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyReleasesActionGetStorageFailure());
			dispatcher.Dispatch(new SpotifyReleasesActionGetFailure(ex.Message) { CompletionSource = action.CompletionSource });
		}*/
	}

	[EffectMethod]
	public async Task GetApi(SpotifyReleaseActionGetApi action, IDispatcher dispatcher)
	{
		try
		{
			// get item from api
			// TODO null
			var releasesApi = await _spotifyReleaseService.GetReleases(action.ReleaseType, action.Artists.List, action.Releases, action.ForceUpdate);
			dispatcher.Dispatch(new SpotifyReleaseActionGetApiSuccess());

			dispatcher.Dispatch(new SpotifyReleaseActionSet(releasesApi));
			dispatcher.Dispatch(new SpotifyReleaseActionSetStorage(releasesApi));
			dispatcher.Dispatch(new SpotifyReleaseActionGetSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyReleaseActionGetApiFailure());
			dispatcher.Dispatch(new SpotifyReleaseActionGetFailure(ex.Message));
		}
	}
	// SET
	[EffectMethod]
	public async Task SetStorage(SpotifyReleaseActionSetStorage action, IDispatcher dispatcher)
	{
		try
		{
			// set item
			var userId = _spotifyUserService.GetUserIdRequired();
			await _dbSpotifyArtistReleaseService.Save(userId, action.Releases);

			dispatcher.Dispatch(new SpotifyReleaseActionSetStorageSuccess());
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyReleaseActionSetStorageFailure(ex.Message));
		}
	}
	[EffectMethod]
	public async Task SetStorageFailure(SpotifyReleaseActionSetStorageFailure action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		//#if DEBUG
		Console.WriteLine(action.ErrorMessage);
		//#endif
	}


	// TODO PERSIST STATE (copy from playlists)
}
