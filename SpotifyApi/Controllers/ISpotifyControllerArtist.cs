using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public interface ISpotifyControllerArtist
{
	Task<ISet<SpotifyArtist>> GetUserFollowedArtists();
}