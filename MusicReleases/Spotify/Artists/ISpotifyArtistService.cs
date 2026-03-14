namespace JakubKastner.MusicReleases.Spotify.Artists;

internal interface ISpotifyArtistService
{
	Task Get(bool forceUpdate = false);
}