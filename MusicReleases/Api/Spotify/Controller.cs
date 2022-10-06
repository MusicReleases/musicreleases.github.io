using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify
{
    public partial class Controller
    {
        public ISpotifyClient? SpotifyClient { set; get; }
    }
}
