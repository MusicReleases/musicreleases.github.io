using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

public class SpotifyReleasesActionSet
{
	public SortedSet<SpotifyAlbum> Releases { get; }

	public SpotifyReleasesActionSet(ISet<SpotifyAlbum> releases)
	{
		Releases = new(releases);
	}
}