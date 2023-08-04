using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerArtist
{
    private readonly ControllerApiArtist _controllerApiArtist;
    private readonly SpotifyUser _user;

    public SpotifyControllerArtist(ControllerApiArtist controllerApiArtist, SpotifyUser user)
    {
        _controllerApiArtist = controllerApiArtist;
        _user = user;
    }

    // get list of user followed artists
    public async Task<SortedSet<SpotifyArtist>> GetUserFollowedArtists()
    {
        return _user.FollowedArtists ??= await _controllerApiArtist.GetUserFollowedArtistsFromApi();
    }
}