using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.SpotifyApi.Services.Api;

internal interface IApiReleaseService
{
	Task<ISet<SpotifyReleaseOld>> GetArtistReleasesFromApi(SpotifyArtist artist, MainReleasesType releaseType);
}