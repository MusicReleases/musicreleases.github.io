using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;

public record SpotifyArtistsState : SpotifyObjectState<SpotifyArtist>
{
	public ISet<SpotifyArtist> NewArtists { get; init; } = new HashSet<SpotifyArtist>();
}