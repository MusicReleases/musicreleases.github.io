using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

public class SpotifyReleasesActionSet(ISet<SpotifyAlbum> releases)
{
	public SortedSet<SpotifyAlbum> Releases { get; } = new(releases);
}