using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;
using JakubKastner.SpotifyApi.Controllers;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasestore;

public class SpotifyReleasesEffects(ISpotifyControllerRelease spotifyControllerRelease)
{
	private readonly ISpotifyControllerRelease _spotifyControllerRelease = spotifyControllerRelease;

	// START - spotify artists success
	/*[EffectMethod]
	public async Task GetFromReleases(SpotifyArtistsActionGetSuccess action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);
		dispatcher.Dispatch(new SpotifyReleasesActionGet(action.ForceUpdate));
	}*/

	// GET
	[EffectMethod]
	public async Task Get(SpotifyReleasesActionGet action, IDispatcher dispatcher)
	{
		// TODO must be task
		await Task.Delay(0);

		//dispatcher.Dispatch(new SpotifyReleasesActionGetStorage(action.ReleaseType, action.ForceUpdate));
	}


	[EffectMethod]
	public async Task GetApi(SpotifyReleasesActionGetApi action, IDispatcher dispatcher)
	{
		try
		{
			// get item from api
			var releasesStorage = action.Releases;
			var releases = await _spotifyControllerRelease.GetAllReleasesFromUserFollowed(action.ReleaseType, releasesStorage, action.ForceUpdate);
			dispatcher.Dispatch(new SpotifyReleasesActionGetApiSuccess());

			dispatcher.Dispatch(new SpotifyReleasesActionSet(releases));
			dispatcher.Dispatch(new SpotifyReleasesActionSetStorage(releases));
		}
		catch (Exception ex)
		{
			dispatcher.Dispatch(new SpotifyReleasesActionGetApiFailure(ex.Message));
		}
	}


	// TODO PERSIST STATE (copy from playlists)
}
