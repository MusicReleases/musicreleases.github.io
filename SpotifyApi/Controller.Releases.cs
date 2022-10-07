using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;
using static JakubKastner.SpotifyApi.Enums;

namespace JakubKastner.SpotifyApi;

public partial class Controller
{
    public async Task<SortedSet<Album>> GetReleases(ReleaseType releaseType, string artistId)
    {
        SortedSet<Album> albums = new();
        var albumsFromApi = await GetReleasesFromApi(artistId);

        if (albumsFromApi == null) return albums;

        foreach (var albumApi in albumsFromApi)
        {
            var album = new Album(albumApi);
            albums.Add(album);
        }

        return albums;
    }

    private async Task<IList<SimpleAlbum>> GetReleasesFromApi(string artistId)
    {
        if (SpotifyClient == null) return null;
        var request = new ArtistsAlbumsRequest
        {
            Limit = 50,
            // TODO Market
        };
        try
        {
            var response = await SpotifyClient.Artists.GetAlbums(artistId, request);
            var albums = await SpotifyClient.PaginateAll(response);
            return albums;
        }
        catch (APIException e)
        {
            // Prints: invalid id
            Console.WriteLine(e.Message);
            // Prints: BadRequest
            Console.WriteLine(e.Response?.StatusCode); // 503 = service unavailable
            await Task.Delay(1000);

        }

        return null;
    }
}
