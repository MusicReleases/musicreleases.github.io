namespace JakubKastner.MusicReleases.Spotify.Base;

internal interface ISpotifyBaseSyncService
{
	Task Get(bool forceUpdate = false);
}