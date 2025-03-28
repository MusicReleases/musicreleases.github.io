using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public interface ISpotifyControllerArtist
{
	Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>> GetUserFollowedArtists(SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>? existingArtists = null, bool forceUpdate = false);
}