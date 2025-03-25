using System.ComponentModel.DataAnnotations;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify.Base;

public class SpotifyGuidEntity : SpotifyEntity
{
	[Key]
	public Guid Guid { get; set; } = Guid.NewGuid();
}