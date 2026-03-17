namespace JakubKastner.MusicReleases.Database.Spotify.Entities.Base;

public interface ISpotifyUserIdLinkEntity : ISpotifyUserIdEntity
{
	string GetLinkedId();
}