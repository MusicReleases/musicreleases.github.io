using Fluxor;
using JakubKastner.SpotifyApi.Controllers;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

public class SpotifyReleasesEffects(ISpotifyControllerRelease spotifyControllerRelease)
{
	private readonly ISpotifyControllerRelease _spotifyControllerRelease = spotifyControllerRelease;

	[EffectMethod(typeof(SpotifyReleasesActionLoad))]
	public async Task LoadReleases(IDispatcher dispatcher)
	{
		var releases = await _spotifyControllerRelease.GetAllUserFollowedArtistsReleases();
		dispatcher.Dispatch(new SpotifyReleasesActionSet(releases));
	}
}
