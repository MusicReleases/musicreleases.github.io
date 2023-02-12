using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Releases;

public class SpotifyReleasesActionSet
{
	public SortedSet<Album> Releases { get; }

	public SpotifyReleasesActionSet(SortedSet<Album> releases)
	{
		Releases = releases;
	}
}