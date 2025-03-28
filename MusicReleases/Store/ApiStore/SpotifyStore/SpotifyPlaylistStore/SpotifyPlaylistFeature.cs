using Fluxor;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistStore;

public class SpotifyPlaylistFeature : Feature<SpotifyPlaylistState>
{
	public override string GetName() => "ApiSpotifyPlaylist";

	protected override SpotifyPlaylistState GetInitialState()
	{
		return new();
	}
}
