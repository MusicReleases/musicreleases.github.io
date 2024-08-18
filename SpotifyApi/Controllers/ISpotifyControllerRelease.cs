using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public interface ISpotifyControllerRelease
{
	Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>> GetReleasesFromArtist(SpotifyEnums.ReleaseType releaseType, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>? existingArtists = null, bool forceUpdate = false);

	//Task<SpotifyUserListArtists<SpotifyRelease>> GetAllReleasesFromUserFollowed(SpotifyEnums.ReleaseType releaseType = SpotifyEnums.ReleaseType.Albums, SpotifyUserListArtists<SpotifyRelease>? existingReleases = null, bool forceUpdate = false);
}