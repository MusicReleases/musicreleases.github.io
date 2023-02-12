using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Releases;

public record SpotifyReleasesState
{
	public bool Initialized { get; init; }
	public bool Loading { get; init; }
	public SortedSet<Album>? Releases { get; init; }
}
