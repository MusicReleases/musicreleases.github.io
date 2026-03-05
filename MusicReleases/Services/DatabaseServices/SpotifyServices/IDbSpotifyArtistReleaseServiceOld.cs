using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyArtistReleaseServiceOld
{
	Task<SpotifyUserList<SpotifyReleaseOld, SpotifyUserListUpdateRelease>?> Get(ISet<SpotifyArtist> artists, string userId);
	Task Save(string userId, SpotifyUserList<SpotifyReleaseOld, SpotifyUserListUpdateRelease> releases);
}