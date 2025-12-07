using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Services.Api;

internal interface IApiArtistServiceOld
{
	Task<ISet<SpotifyArtist>> GetUserFollowedArtistsFromApi();
}