namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyArtistReleaseEntity : SpotifyGuidEntity
{
	public string ReleaseId { get; set; }

	public string ArtistId { get; set; }
}
