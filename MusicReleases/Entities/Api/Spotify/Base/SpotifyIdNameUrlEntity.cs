namespace JakubKastner.MusicReleases.Entities.Api.Spotify.Base;

public class SpotifyIdNameUrlEntity : SpotifyIdNameEntity
{
	public required string UrlApp { get; init; }
	public required string UrlWeb { get; init; }
}
