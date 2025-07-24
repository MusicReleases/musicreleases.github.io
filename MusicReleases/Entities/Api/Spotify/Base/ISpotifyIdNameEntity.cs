namespace JakubKastner.MusicReleases.Entities.Api.Spotify.Base;

public interface ISpotifyIdNameEntity : ISpotifyIdEntity
{
	string Name { get; init; }
}
