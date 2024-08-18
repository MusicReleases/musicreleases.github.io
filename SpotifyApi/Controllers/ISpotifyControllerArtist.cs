using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public interface ISpotifyControllerArtist
{
	Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>> GetUserFollowedArtists(SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>? existingArtists = null, bool forceUpdate = false);
}