using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public interface IDatabaseSpotifyArtistReleaseController
{
	Task<SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>?> Get(ISet<SpotifyArtist> artists, string userId);
	Task Save(string userId, SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease> releases);
}