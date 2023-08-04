using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Playlists;

public class SpotifyPlaylistsActionSet
{
	public SortedSet<SpotifyPlaylist> Playlists { get; }

	public SpotifyPlaylistsActionSet(ISet<SpotifyPlaylist> playlists)
	{
		Playlists = new(playlists);
	}
}