using JakubKastner.SpotifyApi.Tracks;

namespace JakubKastner.MusicReleases.Spotify.Tracks
{
	public interface IDbSpotifyTrackService
	{
		Task<IReadOnlyList<SpotifyTrack>?> GetAll();
		Task<IReadOnlyList<SpotifyTrack>> GetByIds(IEnumerable<string> ids);
		Task<IReadOnlyList<SpotifyTrack>> GetByReleaseId(string releaseId);
		Task Save(IReadOnlyList<SpotifyTrack> tracks);
	}
}