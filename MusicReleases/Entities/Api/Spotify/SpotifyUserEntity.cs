namespace JakubKastner.MusicReleases.Entities.Api.Spotify;

public class SpotifyUserEntity : SpotifyIdNameEntity
{
	public string? Country { get; set; }
	public string? ProfilePictureUrl { get; set; }
	public string? RefreshToken { get; set; }
}