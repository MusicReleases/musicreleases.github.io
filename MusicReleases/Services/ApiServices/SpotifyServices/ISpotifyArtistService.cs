
namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices
{
	public interface ISpotifyArtistService
	{
		void Cancel();
		Task Get(string userId, bool forceUpdate = false);
	}
}