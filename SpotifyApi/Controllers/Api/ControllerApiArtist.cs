using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public class ControllerApiArtist
{
    private readonly SpotifyClient _spotifyClient;

    public ControllerApiArtist(Client client)
    {
        _spotifyClient = client.GetClient();
    }

    public async Task<SortedSet<Artist>> GetUserFollowedArtistsFromApi()
    {
        SortedSet<Artist> artists = new();
        var artistsFromApi = await GetUserFollowedArtistsApi();

        if (artistsFromApi == null) return artists;

        foreach (var artistApi in artistsFromApi)
        {
            var artist = new Artist(artistApi);
            artists.Add(artist);
        }

        return artists;
    }

    private async Task<IList<FullArtist>?> GetUserFollowedArtistsApi()
    {
        var request = new FollowOfCurrentUserRequest(FollowOfCurrentUserRequest.Type.Artist)
        {
            Limit = 50
        };


        var response = await _spotifyClient.Follow.OfCurrentUser(request);
        var artistsAsync = _spotifyClient.Paginate(response.Artists, (s) => s.Artists);

        List<FullArtist> artists = new();

        await foreach (var artist in artistsAsync)
        {
            artists.Add(artist);
        }
        return artists;
    }
}