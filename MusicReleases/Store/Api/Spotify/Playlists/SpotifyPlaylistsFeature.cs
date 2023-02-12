using Fluxor;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Playlists;

public class SpotifyPlaylistsFeature : Feature<SpotifyPlaylistsState>
{
	public override string GetName() => "ApiSpotifyPlaylists";

	protected override SpotifyPlaylistsState GetInitialState()
	{
		return new()
		{
			Initialized = false,
			Loading = false,
			Playlists = new(),
		};
	}
}
