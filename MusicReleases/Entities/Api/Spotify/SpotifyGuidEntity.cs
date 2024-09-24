using System.ComponentModel.DataAnnotations;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyGuidEntity : SpotifyEntity
{
	[Key]
	public Guid Id { get; set; } = Guid.NewGuid();
}
