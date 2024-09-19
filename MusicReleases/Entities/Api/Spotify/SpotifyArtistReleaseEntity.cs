using System.ComponentModel.DataAnnotations;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyArtistReleaseEntity
{
	[Key]
	public Guid Id { get; set; }

	public string ReleaseId { get; set; }

	public string ArtistId { get; set; }
}
