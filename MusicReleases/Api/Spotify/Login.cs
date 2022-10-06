using MusicReleases.Api.Spotify.Objects;
using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify;

public class Login
{
    private readonly Controller _controller;
    private readonly IUser _user;

    public Login(Controller controller, IUser user)
    {
        _controller = controller;
        _user = user;
    }

    public static Uri GetLoginUrl(Uri currentUrl)
    {
        ICollection<string>? scope = new[] { Scopes.UserLibraryRead, Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative, Scopes.UserFollowRead };
        const string appId = "c5f5fe8e454e486aae846c51a68ddd98";

        var loginRequest = new LoginRequest(currentUrl, appId, LoginRequest.ResponseType.Token)
        {
            Scope = scope
        };
        return loginRequest.ToUri();
    }

    public async Task LoginUser(string url)
    {
        await LoginUser(new Uri(url));
    }

    public async Task LoginUser(Uri url)
    {
        _controller.LoginUser(url);
        await _user.SetUser();
    }
}
