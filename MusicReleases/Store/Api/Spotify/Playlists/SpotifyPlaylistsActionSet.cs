using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Playlists;

public class SpotifyPlaylistsActionSet
{
	public SortedSet<Playlist> Playlists { get; }

	public SpotifyPlaylistsActionSet(SortedSet<Playlist> playlists)
	{
		Playlists = playlists;
	}
}