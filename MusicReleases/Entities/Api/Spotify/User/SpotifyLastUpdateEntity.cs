using JakubKastner.MusicReleases.Entities.Api.Spotify.Base;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify.User;

public class SpotifyLastUpdateEntity : SpotifyEntity
{
	public string? UserId { get; set; }

	public DateTime? User { get; set; }

	public DateTime? Playlists { get; set; }
	public DateTime? Artists { get; set; }

	public DateTime? ReleasesAlbums { get; set; }
	public DateTime? ReleasesTracks { get; set; }
	public DateTime? ReleasesAppears { get; set; }
	public DateTime? ReleasesCompilations { get; set; }
	public DateTime? ReleasesPodcasts { get; set; }
}
