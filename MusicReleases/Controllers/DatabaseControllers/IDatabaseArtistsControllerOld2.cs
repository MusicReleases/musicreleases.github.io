using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Controllers.DatabaseControllers;

public interface IDatabaseArtistsControllerOld2
{
	Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>?> GetArtists();
	Task SaveArtists(SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists> artists);
}
