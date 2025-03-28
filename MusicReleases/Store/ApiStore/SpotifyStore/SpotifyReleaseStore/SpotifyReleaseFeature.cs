using Fluxor;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleaseStore;

public class SpotifyReleaseFeature : Feature<SpotifyReleaseState>
{
	public override string GetName() => "ApiSpotifyRelease";

	protected override SpotifyReleaseState GetInitialState()
	{
		return new();
	}
}
