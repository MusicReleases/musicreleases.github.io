using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.Api.Spotify.Artists;

public record SpotifyArtistsState
{
	public bool Initialized { get; init; }
	public bool Loading { get; init; }
	public SortedSet<Artist>? Artists { get; init; }
}
