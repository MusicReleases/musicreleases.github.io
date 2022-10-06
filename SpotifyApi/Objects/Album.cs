using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class Album
{
    public string Id { get; private set; }
    public string Name { get; private set; }

    public HashSet<Artist> Artists { get; private set; }

    // TODO artists - GetArtists
    public Album(SimpleAlbum simpleAlbum)
    {
        Id = simpleAlbum.Id;
        Name = simpleAlbum.Name;
        Artists = new();
        //Artists = Controller.GetArtists(simpleAlbum.Artists);
    }
    public Album(FullAlbum fullAlbum)
    {
        Id = fullAlbum.Id;
        Name = fullAlbum.Name;
        Artists = new();
        //Artists = Controller.GetArtists(fullAlbum.Artists);
    }
    public Album(SimpleShow simpleShow)
    {
        Id = simpleShow.Id;
        Name = simpleShow.Name;
        Artists = new()
        {
            new(id: "0", name: simpleShow.Publisher)
        };
    }
}
