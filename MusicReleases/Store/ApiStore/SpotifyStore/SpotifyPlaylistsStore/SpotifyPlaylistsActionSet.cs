using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;

public class SpotifyPlaylistsActionSet(ISet<SpotifyPlaylist> playlists)
{
	public SortedSet<SpotifyPlaylist> Playlists { get; } = new(playlists);
}