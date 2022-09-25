using MusicReleases.Api.Spotify.Objects;
using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify
{
    public partial class Controller
    {
        public HashSet<Artist> GetArtists(List<SimpleArtist> simpleArtistsList)
        {
            HashSet<Artist> artistsList = new();
            foreach (var artist in simpleArtistsList)
            {
                Artist newArtist = new(artist);
                artistsList.Add(newArtist);
            }
            return artistsList;
        }

        public async Task<SortedSet<Artist>?> GetArtists()
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
        private async Task<IList<FullArtist>?> GetArtistsApi()
        {
            if (_spotifyUser.Client == null) return null;

            List<FullArtist> artists = new();

            var request = new FollowOfCurrentUserRequest(FollowOfCurrentUserRequest.Type.Artist)
            {
                Limit = 50
            };

            var response = await _spotifyUser.Client.Follow.OfCurrentUser(request);
            await foreach (var artist in _spotifyUser.Client.Paginate(response.Artists, (s) => s.Artists))
            {
                artists.Add(artist);
            }

            return artists;
        }
    }
}
