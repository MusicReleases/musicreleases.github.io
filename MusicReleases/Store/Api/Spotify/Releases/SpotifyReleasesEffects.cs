using Fluxor;
using JakubKastner.SpotifyApi.Controllers;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Releases;

public class SpotifyReleasesEffects
{
	private readonly ISpotifyControllerRelease _spotifyControllerRelease;

	public SpotifyReleasesEffects(ISpotifyControllerRelease spotifyControllerRelease)
	{
		_spotifyControllerRelease = spotifyControllerRelease;
	}

	[EffectMethod(typeof(SpotifyReleasesActionLoad))]
	public async Task LoadReleases(IDispatcher dispatcher)
	{
		var releases = await _spotifyControllerRelease.GetAllUserFollowedArtistsReleases();
		dispatcher.Dispatch(new SpotifyReleasesActionSet(releases));
	}
}
