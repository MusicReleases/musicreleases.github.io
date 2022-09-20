using MusicReleases.Api.Spotify.Objects;
using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify
{
    public static partial class Controller
    {
        public static HashSet<Artist> GetArtists(List<SimpleArtist> simpleArtistsList)
        {
            HashSet<Artist> artistsList = new();
            foreach (var artist in simpleArtistsList)
            {
                Artist newArtist = new(artist);
                artistsList.Add(newArtist);
            }
            return artistsList;
        }
    }
}
