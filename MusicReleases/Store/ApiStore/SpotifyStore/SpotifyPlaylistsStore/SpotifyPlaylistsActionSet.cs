using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

public class SpotifyPlaylistsActionSet
{
	public SortedSet<SpotifyPlaylist> Playlists { get; }

	public SpotifyPlaylistsActionSet(ISet<SpotifyPlaylist> playlists)
	{
		Playlists = new(playlists);
	}
}