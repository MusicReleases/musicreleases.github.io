namespace JakubKastner.MusicReleases.Database.Spotify.Services;

public interface IDbSpotifyUserArtistService
{
	Task<IReadOnlyCollection<string>> GetFollowedIds(string userId, CancellationToken ct);
	Task SetFollowed(string userId, IEnumerable<string> artistIds, CancellationToken ct);
}