using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.SpotifyApi.Services;

public interface ISpotifyArtistService
{
	Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>> GetUserFollowedArtists(SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>? existingArtists = null, bool forceUpdate = false);
}