using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Services.Api;

internal interface IApiReleaseService
{
	Task<ISet<SpotifyRelease>> GetArtistReleasesFromApi(SpotifyArtist artist, SpotifyEnums.ReleaseType releaseType);
}