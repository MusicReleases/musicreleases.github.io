namespace JakubKastner.MusicReleases.Services
{
	internal interface ISpotifyBaseSyncService
	{
		Task Get(bool forceUpdate = false);
	}
}