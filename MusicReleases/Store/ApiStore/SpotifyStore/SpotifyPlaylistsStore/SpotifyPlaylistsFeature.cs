using Fluxor;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

public class SpotifyPlaylistsFeature : Feature<SpotifyPlaylistsState>
{
	public override string GetName() => "ApiSpotifyPlaylists";

	protected override SpotifyPlaylistsState GetInitialState()
	{
		return new();
	}
}
