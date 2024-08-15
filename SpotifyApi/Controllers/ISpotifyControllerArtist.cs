using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public interface ISpotifyControllerArtist
{
	Task<SpotifyUserList<SpotifyArtist>> GetUserFollowedArtists(SpotifyUserList<SpotifyArtist>? existingArtists = null, bool forceUpdate = false);
}