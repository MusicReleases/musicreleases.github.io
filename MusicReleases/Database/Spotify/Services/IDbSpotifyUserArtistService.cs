namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public interface IDbSpotifyUserArtistService
{
	Task<IReadOnlyCollection<string>> GetFollowedIds(string userId);
	Task SetFollowed(string userId, IEnumerable<string> artistIds);
}