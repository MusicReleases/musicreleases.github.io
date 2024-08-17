using Fluxor;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;

public class SpotifyPlaylistsTracksFeature : Feature<SpotifyPlaylistsTracksState>
{
	public override string GetName() => "ApiSpotifyPlaylistsTracks";

	protected override SpotifyPlaylistsTracksState GetInitialState()
	{
		return new();
	}
}
