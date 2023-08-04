using JakubKastner.SpotifyApi.Controllers.Api;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerUser
{
    private readonly SpotifyClient _client;
    private readonly ControllerApiUser _controllerApiUser;

    public SpotifyControllerUser(SpotifyClient client, ControllerApiUser controllerApiUser)
    {
        _client = client;
        _controllerApiUser = controllerApiUser;
    }

    public Uri GetLoginUrl(Uri currentUrl)
    {
        ICollection<string>? scope = new[]
        {
            Scopes.UserLibraryRead,
            Scopes.PlaylistReadPrivate,
            Scopes.PlaylistReadCollaborative,
            Scopes.UserFollowRead,
        };
        const string appId = "67bbd538e581437597ae4574431682df";

        var loginRequest = new LoginRequest(currentUrl, appId, LoginRequest.ResponseType.Token)
        {
            Scope = scope
        };
        return loginRequest.ToUri();
    }

    public bool IsLoggedIn()
    {
        return _client.IsInicialized();
    }

    public async Task<bool> LoginUser(string currentUrl)
    {
        return await _controllerApiUser.LoginUser(currentUrl) != null;
    }
}
