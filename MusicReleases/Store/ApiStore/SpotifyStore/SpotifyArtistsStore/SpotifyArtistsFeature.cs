using Fluxor;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;

public class SpotifyArtistsFeature : Feature<SpotifyArtistsState>
{
	public override string GetName() => "ApiSpotifyArtists";

	protected override SpotifyArtistsState GetInitialState()
	{
		return new()
		{
			Initialized = false,
			Loading = false,
			Artists = [],
		};
	}
}
