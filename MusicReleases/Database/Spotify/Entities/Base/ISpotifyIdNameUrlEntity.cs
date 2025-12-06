namespace JakubKastner.MusicReleases.Database.Spotify.Entities.Base;

public interface ISpotifyIdNameUrlEntity : ISpotifyIdNameEntity
{
	string UrlApp { get; init; }
	string UrlWeb { get; init; }
}
