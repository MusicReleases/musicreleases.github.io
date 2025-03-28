using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistStore;

public record SpotifyArtistState : SpotifyObjectState<SpotifyArtist, SpotifyUserListUpdateMain>
{
	public ISet<SpotifyArtist> NewArtists { get; init; } = new HashSet<SpotifyArtist>();
}