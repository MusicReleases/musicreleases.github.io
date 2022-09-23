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

        public static async Task<SortedSet<Artist>?> GetArtists()
        {
            SortedSet<Artist> artists = new();
            var artistsFromApi = await GetArtistsApi();

            if (artistsFromApi == null) return artists;

            foreach (var artistApi in artistsFromApi)
            {
                var artist = new Artist(artistApi);
                artists.Add(artist);
            }

            return artists;
        }
        private static async Task<IList<FullArtist>?> GetArtistsApi()
        {
            if (Main.Client == null) return null;

            List<FullArtist> artists = new();

            var request = new FollowOfCurrentUserRequest(FollowOfCurrentUserRequest.Type.Artist)
            {
                Limit = 50
            };

            var response = await Main.Client.Follow.OfCurrentUser(request);
            await foreach (var artist in Main.Client.Paginate(response.Artists, (s) => s.Artists))
            {
                artists.Add(artist);
            }

            return artists;
        }
    }
}
