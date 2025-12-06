namespace JakubKastner.MusicReleases.Database.Spotify.Entities.Base;

public interface ISpotifyIdNameEntity : ISpotifyIdEntity
{
	string Name { get; init; }
}
