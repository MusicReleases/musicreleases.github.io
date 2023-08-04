using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Releases;

public class SpotifyReleasesActionSet
{
	public SortedSet<SpotifyAlbum> Releases { get; }

	public SpotifyReleasesActionSet(ISet<SpotifyAlbum> releases)
	{
		Releases = new(releases);
	}
}