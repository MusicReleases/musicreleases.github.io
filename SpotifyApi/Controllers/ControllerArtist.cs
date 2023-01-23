using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public class ControllerArtist
{
    private readonly ControllerApiArtist _controllerApiArtist;
    private readonly User _user;

    public ControllerArtist(ControllerApiArtist controllerApiArtist, User user)
    {
        _controllerApiArtist = controllerApiArtist;
        _user = user;
    }

    // get list of user followed artists
    public async Task<SortedSet<Artist>> GetUserFollowedArtists()
    {
        return _user.FollowedArtists ??= await _controllerApiArtist.GetUserFollowedArtistsFromApi();
    }
}