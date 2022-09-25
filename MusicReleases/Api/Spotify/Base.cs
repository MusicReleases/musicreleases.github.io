using MusicReleases.Api.Spotify.Objects;
using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify;

// TODO remove class
public class Base
{
    /*public const string AppId = "c5f5fe8e454e486aae846c51a68ddd98";
    public static readonly ICollection<string>? Scope = new[] { Scopes.UserLibraryRead, Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative, Scopes.UserFollowRead };

    public SpotifyClient? Client { get; private set; }

    /// <summary>
    /// Spotify logged in user
    /// </summary>
    public User? User { get; private set; }

    public Base()
    {
        
    }

    /// <summary>
    /// get user from spotify api and store to values
    /// </summary>
    /// <param name="url"></param>
    /// <returns></returns>
    public async Task SetUser(Uri url)
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
        Client = new(accessToken);

        var privateUser = await Client.UserProfile.Current();
        User = new(privateUser);

        if (!urlParameters.ContainsKey("expires_in")) return;
        var accessTokenExpires = urlParameters["expires_in"];
    }*/
}
