using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi;

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

    public async Task<SortedSet<Artist>> GetUserArtists()
    {
        SortedSet<Artist> artists = new();
        var artistsFromApi = await GetUserArtistsFromApi();

        if (artistsFromApi == null) return artists;

        foreach (var artistApi in artistsFromApi)
        {
            var artist = new Artist(artistApi);
            artists.Add(artist);
        }

        return artists;
    }

    private async Task<IList<FullArtist>?> GetUserArtistsFromApi()
    {
        if (SpotifyClient == null) return null;

        List<FullArtist> artists = new();

        var request = new FollowOfCurrentUserRequest(FollowOfCurrentUserRequest.Type.Artist)
        {
            Limit = 50
        };

        var response = await SpotifyClient.Follow.OfCurrentUser(request);
        await foreach (var artist in SpotifyClient.Paginate(response.Artists, (s) => s.Artists))
        {
            artists.Add(artist);
        }

        return artists;
    }
}
