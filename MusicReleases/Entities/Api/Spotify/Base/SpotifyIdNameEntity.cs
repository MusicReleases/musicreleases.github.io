using System.ComponentModel.DataAnnotations;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify.Base;

public class SpotifyIdNameEntity : SpotifyEntity
{
	[Key]
	public new string? Id { get; set; }
	public string? Name { get; set; }
}
