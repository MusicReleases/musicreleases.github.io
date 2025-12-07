
namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

public interface IDbSpotifyUserArtistService
{
	Task<IReadOnlyCollection<string>> GetFollowedIds(string userId);
	Task SetFollowed(string userId, IEnumerable<string> artistIds);
}