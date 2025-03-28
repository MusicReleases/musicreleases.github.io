using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Services;

public interface ISpotifyReleaseService
{
	Task<SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>> GetReleases(ReleaseType releaseType, ISet<SpotifyArtist> artists, SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? existingReleases = null, bool forceUpdate = false);
}