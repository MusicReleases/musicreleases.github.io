using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Services.Api;

internal interface IApiArtistService
{
	Task<ISet<SpotifyArtist>> GetUserFollowedArtistsFromApi();
}