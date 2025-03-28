using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyArtistReleaseService
{
	Task<SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>?> Get(ISet<SpotifyArtist> artists, string userId);
	Task Save(string userId, SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease> releases);
}