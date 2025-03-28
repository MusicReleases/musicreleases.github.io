using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;

public record SpotifyReleasesState : SpotifyObjectState<SpotifyRelease, SpotifyUserListUpdateRelease>
{
	public ISet<SpotifyRelease> NewReleases { get; init; } = new HashSet<SpotifyRelease>();
}
