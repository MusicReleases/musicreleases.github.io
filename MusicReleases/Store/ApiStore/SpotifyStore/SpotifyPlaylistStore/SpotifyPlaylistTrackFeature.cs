using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistStore;

public class SpotifyPlaylistTrackFeature : Feature<SpotifyPlaylistTrackState>
{
	public override string GetName() => "ApiSpotifyPlaylistTrack";

	protected override SpotifyPlaylistTrackState GetInitialState()
	{
		return new();
	}
}
