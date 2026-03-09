namespace JakubKastner.MusicReleases.Database.Spotify.Services
{
	public interface IDbSpotifyArtistTrackService
	{
		Task AddArtistTrack(string artistId, string releaseId);
		Task DeleteAllForArtist(string artistId);
		Task<HashSet<string>> GetArtistTrackIds(string artistId);
		Task SetArtistTracks(string artistId, IEnumerable<string> trackApiIdsEnumerable);
	}
}