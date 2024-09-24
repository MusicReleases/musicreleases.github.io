namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyLastUpdateEntity : SpotifyGuidEntity
{
	public string UserId { get; set; }

	public DateTime? User { get; set; }

	public DateTime? Playlists { get; set; }
	public DateTime? Artists { get; set; }

	public DateTime? ReleasesAlbums { get; set; }
	public DateTime? ReleasesTracks { get; set; }
	public DateTime? ReleasesAppears { get; set; }
	public DateTime? ReleasesCompilations { get; set; }
	public DateTime? ReleasesPodcasts { get; set; }
}
