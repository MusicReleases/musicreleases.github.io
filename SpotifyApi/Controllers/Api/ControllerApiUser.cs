using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public class ControllerApiUser
{
    private readonly Client _client;

    public ControllerApiUser(Client client)
    {
        _client = client;
    }

    public bool LoginUser(string url)
    {
        return LoginUser(new Uri(url));
    }

    public bool LoginUser(Uri url)
    {
        // get url parameters
        var maxLen = Math.Min(1, url.Fragment.Length);
        Dictionary<string, string> urlParameters = url.Fragment[maxLen..]?
          .Split("&", StringSplitOptions.RemoveEmptyEntries)?
          .Select(param => param.Split("=", StringSplitOptions.RemoveEmptyEntries))?
          .ToDictionary(param => param[0], param => param[1]) ?? new Dictionary<string, string>();

        // get user from access token
        var loggedIn = urlParameters.ContainsKey("access_token");
        if (!loggedIn) return false;

        var accessToken = urlParameters["access_token"];
        if (string.IsNullOrEmpty(accessToken)) return false;
        _client.Init(accessToken);

        if (!urlParameters.ContainsKey("expires_in")) return false;
        var accessTokenExpires = urlParameters["expires_in"];

        return true;
    }

    public async Task<PrivateUser> GetLoggedInUser()
    {
        var spotifyClient = _client.GetClient();
        return await spotifyClient.UserProfile.Current();
    }
}