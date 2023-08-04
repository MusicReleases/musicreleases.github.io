using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public interface IControllerApiArtist
{
	Task<ISet<SpotifyArtist>> GetUserFollowedArtistsFromApi();
}