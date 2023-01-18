using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.SpotifyApi.Enums;

namespace JakubKastner.SpotifyApi.Controllers;

public class ControllerUser
{
    private readonly Controller _controller;
    private readonly User _user;
    private readonly Client _spotifyClient;

    public ControllerUser(Controller controller, User user, Client spotifyClient)
    {
        _controller = controller;
        _user = user;
        _spotifyClient = spotifyClient;
    }

    public bool IsLoggedIn() => _spotifyClient.IsInicialized();

    // get list of user followed artists
    public async Task<SortedSet<Artist>> GetArtists()
    {
        if (_user.FollowedArtists == null)
        {
            _user.FollowedArtists = await _controller.GetUserArtists();
        }
        return _user.FollowedArtists;
    }

    // get releases
    public async Task<SortedSet<Album>> GetReleases(ReleaseType releaseType = ReleaseType.Albums)
    {
        var artists = await GetArtists();
        var albums = new SortedSet<Album>();

        foreach (var artist in artists)
        {
            var albumsFromApi = await _controller.GetReleases(releaseType, artist.Id);
            // TODO try again
            if (albumsFromApi == null)
            {
                albumsFromApi = await _controller.GetReleases(releaseType, artist.Id);
            }
            albums.UnionWith(albumsFromApi);
        }

        return albums;
    }
}
