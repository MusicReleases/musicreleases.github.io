using MusicReleases.Api.Spotify.Objects;
using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify
{
    public static class Main
    {
        public const string AppId = "c5f5fe8e454e486aae846c51a68ddd98";
        public static readonly ICollection<string>? Scope = new[] { Scopes.UserLibraryRead, Scopes.PlaylistReadPrivate, Scopes.PlaylistReadCollaborative };

        public static SpotifyClient? Client { get; private set; }

        /// <summary>
        /// Spotify logged in user
        /// </summary>
        public static User? User { get; private set; }

        /// <summary>
        /// get user from spotify api and store to values
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task SetUser(Uri url)
        {
            // get url parameters
            var maxLen = Math.Min(1, url.Fragment.Length);
            Dictionary<string, string> urlParameters = url.Fragment[maxLen..]?
              .Split("&", StringSplitOptions.RemoveEmptyEntries)?
              .Select(param => param.Split("=", StringSplitOptions.RemoveEmptyEntries))?
              .ToDictionary(param => param[0], param => param[1]) ?? new Dictionary<string, string>();

            // get user from access token
            bool loggedIn = urlParameters.ContainsKey("access_token");
            if (!loggedIn) return;

            Client = new(urlParameters["access_token"]);
            var privateUser = await Client.UserProfile.Current();
            User = new(user: privateUser);

            /*var playlists = await Controller.GetPlaylists();
            User.Playlists = playlists;*/
        }
    }
}
