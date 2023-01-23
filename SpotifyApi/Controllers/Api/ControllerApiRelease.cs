using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;
using static JakubKastner.SpotifyApi.Enums;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public class ControllerApiRelease
{
    private readonly SpotifyClient _spotifyClient;

    public ControllerApiRelease(Client client)
    {
        _spotifyClient = client.GetClient();
    }

    public async Task<SortedSet<Album>> GetArtistReleasesFromApi(string artistId, ReleaseType releaseType)
    {
        SortedSet<Album> albums = new();
        var albumsFromApi = await GetArtistReleasesApi(artistId);

        if (albumsFromApi == null) return albums;

        foreach (var albumApi in albumsFromApi)
        {
            var album = new Album(albumApi);
            albums.Add(album);
        }

        return albums;
    }

    private async Task<IList<SimpleAlbum>?> GetArtistReleasesApi(string artistId)
    {
        var request = new ArtistsAlbumsRequest
        {
            Limit = 50,
            // TODO Market
        };

        try
        {
            var response = await _spotifyClient.Artists.GetAlbums(artistId, request);
            var albums = await _spotifyClient.PaginateAll(response);
            return albums;
        }
        catch (APIException e)
        {
            // TODO try again
            // Prints: invalid id
            Console.WriteLine(e.Message);
            // Prints: BadRequest
            Console.WriteLine(e.Response?.StatusCode); // 503 = service unavailable
            await Task.Delay(1000);
        }

        // TODO null
        return null;
    }
}