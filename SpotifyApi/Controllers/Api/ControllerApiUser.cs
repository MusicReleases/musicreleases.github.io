using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public class ControllerApiUser
{
    private readonly Client _client;
    private readonly User _user;

    public ControllerApiUser(Client client, User user)
    {
        _client = client;
        _user = user;
    }

    public async Task<User?> LoginUser(string url)
    {
        return await LoginUser(new Uri(url));
    }

    public async Task<User?> LoginUser(Uri url)
    {
        // get url parameters
        var urlParameters = GetUrlParameters(url);

        // get user from access token
        var loggedIn = urlParameters.ContainsKey("access_token");
        if (!loggedIn)
        {
            return null;
        }

        var accessToken = urlParameters["access_token"];
        if (string.IsNullOrEmpty(accessToken))
        {
            return null;
        }
        _client.Init(accessToken);

        if (!urlParameters.ContainsKey("expires_in"))
        {
            return null;
        }
        var accessTokenExpires = urlParameters["expires_in"];

        // get user info
        var userApi = await GetLoggedInUser();
        _user.Id = userApi.Id;

        return GetUser(userApi);
    }

    public User GetUser(PrivateUser userApi)
    {
        return new()
        {
            Id = userApi.Id,
            Name = userApi.DisplayName
        };
    }

    private Dictionary<string, string> GetUrlParameters(Uri url)
    {
        var maxLen = Math.Min(1, url.Fragment.Length);
        var urlParameters = url.Fragment[maxLen..]?
          .Split("&", StringSplitOptions.RemoveEmptyEntries)?
          .Select(param => param.Split("=", StringSplitOptions.RemoveEmptyEntries))?
          .ToDictionary(param => param[0], param => param[1]) ?? new Dictionary<string, string>();

        return urlParameters;
    }

    public async Task<PrivateUser> GetLoggedInUser()
    {
        var spotifyClient = _client.GetClient();
        return await spotifyClient.UserProfile.Current();
    }
}