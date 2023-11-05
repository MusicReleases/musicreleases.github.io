using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;

public record SpotifyArtistsState
{
	public bool Initialized { get; init; }
	public bool Loading { get; init; }
	public SortedSet<SpotifyArtist>? Artists { get; init; }
}
