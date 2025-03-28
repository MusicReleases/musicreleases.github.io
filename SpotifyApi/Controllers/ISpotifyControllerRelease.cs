using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Controllers;

public interface ISpotifyControllerRelease
{
	Task<SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>> GetReleases(ReleaseType releaseType, ISet<SpotifyArtist> artists, SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? existingReleases = null, bool forceUpdate = false);
}