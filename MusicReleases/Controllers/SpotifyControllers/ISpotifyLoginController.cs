using Microsoft.Extensions.Primitives;

namespace JakubKastner.MusicReleases.Controllers.SpotifyControllers
{
	public interface ISpotifyLoginController
	{
		Task<bool> IsUserSaved();
		Task LoginUser();
		Task SetUser(StringValues code);
	}
}