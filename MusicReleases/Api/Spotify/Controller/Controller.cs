using MusicReleases.Api.Spotify.Objects;
using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify
{
    public partial class Controller
    {
        // TODO remove dependecy injection looped
        /*private IUser _spotifyUser;

        public Controller(IUser spotifyUser)
        {
            _spotifyUser = spotifyUser;
        }*/


        public ISpotifyClient? SpotifyClient { set; get; }

        public Controller()
        {
        }

    }
}
