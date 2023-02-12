using Fluxor;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Releases;

public class SpotifyReleasesFeature : Feature<SpotifyReleasesState>
{
	public override string GetName()
	{
		return "ApiSpotifyReleases";
	}

	protected override SpotifyReleasesState GetInitialState()
	{
		return new()
		{
			Initialized = false,
			Loading = false,
			Releases = new(),
		};
	}
}
