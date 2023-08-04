using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public interface IControllerApiRelease
{
	Task<ISet<SpotifyAlbum>> GetArtistReleasesFromApi(string artistId, SpotifyEnums.ReleaseType releaseType);
}