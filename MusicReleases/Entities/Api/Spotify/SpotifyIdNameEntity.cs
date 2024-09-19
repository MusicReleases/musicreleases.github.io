using System.ComponentModel.DataAnnotations;

namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyIdNameEntity
{
	[Key]
	public string Id { get; set; }
	public string Name { get; set; }
}
