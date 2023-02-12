using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Playlists;

public class SpotifyPlaylistsActionSet
{
	public HashSet<Playlist> Playlists { get; }

	public SpotifyPlaylistsActionSet(HashSet<Playlist> playlists)
	{
		Playlists = playlists;
	}
}