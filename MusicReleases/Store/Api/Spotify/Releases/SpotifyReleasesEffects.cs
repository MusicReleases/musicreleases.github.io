using Fluxor;
using JakubKastner.SpotifyApi.Controllers;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Releases;

public class SpotifyReleasesEffects
{
	private readonly ControllerRelease _spotifyControllerRelease;

	public SpotifyReleasesEffects(ControllerRelease spotifyControllerRelease)
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
