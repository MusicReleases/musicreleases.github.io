using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Playlists;

public class SpotifyPlaylistsActionSet
{
	public SortedSet<SpotifyPlaylist> Playlists { get; }

	public SpotifyPlaylistsActionSet(SortedSet<SpotifyPlaylist> playlists)
	{
		Playlists = playlists;
	}
}