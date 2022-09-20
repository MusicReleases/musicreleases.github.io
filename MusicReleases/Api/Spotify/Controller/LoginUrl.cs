using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify
{
    public static partial class Controller
    {
        public static Uri GetLoginUrl(Uri currentUrl)
        {
            var loginRequest = new LoginRequest(currentUrl, Main.AppId, LoginRequest.ResponseType.Token)
            {
                Scope = Main.Scope
            };
            return loginRequest.ToUri();
        }
    }
}
