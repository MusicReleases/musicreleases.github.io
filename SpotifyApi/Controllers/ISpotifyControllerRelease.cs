using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public interface ISpotifyControllerRelease
{
	Task<ISet<SpotifyAlbum>> GetAllUserFollowedArtistsReleases(SpotifyEnums.ReleaseType releaseType = SpotifyEnums.ReleaseType.Albums);
}