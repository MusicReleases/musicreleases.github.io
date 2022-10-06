using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify;

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


    public void LoginUser(Uri url)
    {
        // get url parameters
        var maxLen = Math.Min(1, url.Fragment.Length);
        Dictionary<string, string> urlParameters = url.Fragment[maxLen..]?
          .Split("&", StringSplitOptions.RemoveEmptyEntries)?
          .Select(param => param.Split("=", StringSplitOptions.RemoveEmptyEntries))?
          .ToDictionary(param => param[0], param => param[1]) ?? new Dictionary<string, string>();

        // get user from access token
        var loggedIn = urlParameters.ContainsKey("access_token");
        if (!loggedIn) return;

        var accessToken = urlParameters["access_token"];
        SpotifyClient = new SpotifyClient(accessToken);


        if (!urlParameters.ContainsKey("expires_in")) return;
        var accessTokenExpires = urlParameters["expires_in"];
    }

    public async Task<PrivateUser?> GetLoggedInUser()
    {
        if (SpotifyClient == null) return null;
        return await SpotifyClient.UserProfile.Current();
    }
}