using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices
{
	public interface IDbSpotifyTrackService
	{
		Task<IReadOnlyList<SpotifyTrack>?> GetAll();
		Task<IReadOnlyList<SpotifyTrack>> GetByIds(IEnumerable<string> ids);
		Task<IReadOnlyList<SpotifyTrack>> GetByReleaseId(string releaseId);
		Task Save(IReadOnlyList<SpotifyTrack> tracks);
	}
}