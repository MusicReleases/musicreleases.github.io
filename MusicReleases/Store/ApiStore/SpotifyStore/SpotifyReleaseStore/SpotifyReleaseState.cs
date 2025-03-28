using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleaseStore;

public record SpotifyReleaseState : SpotifyObjectState<SpotifyRelease, SpotifyUserListUpdateRelease>
{
	public ISet<SpotifyRelease> NewReleases { get; init; } = new HashSet<SpotifyRelease>();
}
