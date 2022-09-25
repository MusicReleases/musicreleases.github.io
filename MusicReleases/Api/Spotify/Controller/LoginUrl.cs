using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify
{
    public partial class Controller
    {
        public Uri GetLoginUrl(Uri currentUrl)
        {
            ICollection<string>? scope = new[] { Scopes.UserLibraryRead, Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative, Scopes.UserFollowRead };
            const string appId = "c5f5fe8e454e486aae846c51a68ddd98";

            var loginRequest = new LoginRequest(currentUrl, appId, LoginRequest.ResponseType.Token)
            {
                Scope = scope
            };
            return loginRequest.ToUri();
        }
    }
}
