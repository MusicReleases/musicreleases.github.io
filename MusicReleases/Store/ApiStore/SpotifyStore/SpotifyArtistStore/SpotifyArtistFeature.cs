using Fluxor;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistStore;

public class SpotifyArtistFeature : Feature<SpotifyArtistState>
{
	public override string GetName() => "ApiSpotifyArtist";

	protected override SpotifyArtistState GetInitialState()
	{
		return new();
	}
}
