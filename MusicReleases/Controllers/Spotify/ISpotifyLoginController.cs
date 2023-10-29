using Microsoft.Extensions.Primitives;

namespace JakubKastner.MusicReleases.Controllers.Spotify
{
	public interface ISpotifyLoginController
	{
		Task<bool> IsUserSaved();
		Task LoginUser();
		Task SetUser(StringValues code);
	}
}