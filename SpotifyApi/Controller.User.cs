using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi;

public partial class Controller
{
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