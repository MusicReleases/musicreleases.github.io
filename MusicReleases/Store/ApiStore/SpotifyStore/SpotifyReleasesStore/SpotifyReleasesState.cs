using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

public record SpotifyReleasesState
{
	public bool Initialized { get; init; }
	public bool Loading { get; init; }
	public SortedSet<SpotifyRelease>? Releases { get; init; }
}
