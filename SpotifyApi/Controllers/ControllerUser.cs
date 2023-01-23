using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers;

public class ControllerUser
{
    private readonly Client _client;

    public ControllerUser(Client client)
    {
        _client = client;
    }

    public Uri GetLoginUrl(Uri currentUrl)
    {
        ICollection<string>? scope = new[] { Scopes.UserLibraryRead, Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative, Scopes.UserFollowRead };
        const string appId = "67bbd538e581437597ae4574431682df";

        var loginRequest = new LoginRequest(currentUrl, appId, LoginRequest.ResponseType.Token)
        {
            Scope = scope
        };
        return loginRequest.ToUri();
    }

    public bool IsLoggedIn() => _client.IsInicialized();
}
