using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public class ControllerApiArtist
{
    private readonly Client _client;

    public ControllerApiArtist(Client client)
    {
        _client = client;
    }

    public async Task<SortedSet<Artist>> GetUserFollowedArtistsFromApi()
    {
        SortedSet<Artist> artists = new();
        var artistsFromApi = await GetUserFollowedArtistsApi();

        if (artistsFromApi == null) return artists;

        foreach (var artistApi in artistsFromApi)
        {
            Artist artist = new(artistApi);
            artists.Add(artist);
        }

        return artists;
    }

    private async Task<IList<FullArtist>?> GetUserFollowedArtistsApi()
    {
        var request = new FollowOfCurrentUserRequest(FollowOfCurrentUserRequest.Type.Artist)
        {
            Limit = ApiRequestLimit.UserFollowedArtists,
        };
        var spotifyClient = _client.GetClient();
        var response = await spotifyClient.Follow.OfCurrentUser(request);
        var artistsAsync = spotifyClient.Paginate(response.Artists, (s) => s.Artists);

        List<FullArtist> artists = new();

        await foreach (var artist in artistsAsync)
        {
            artists.Add(artist);
        }
        return artists;
    }
}