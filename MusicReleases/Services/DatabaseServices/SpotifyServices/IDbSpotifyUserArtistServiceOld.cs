using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyUserArtistServiceOld
{
	Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>?> Get(string userId);
	Task Save(string userId, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain> artists);
	Task Delete(string userId);
}