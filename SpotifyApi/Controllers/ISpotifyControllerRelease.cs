using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public interface ISpotifyControllerRelease
{
	Task<SpotifyUserListReleases<SpotifyRelease>> GetAllReleasesFromUserFollowed(SpotifyEnums.ReleaseType releaseType = SpotifyEnums.ReleaseType.Albums, SpotifyUserListReleases<SpotifyRelease>? existingReleases = null, bool forceUpdate = false);
}