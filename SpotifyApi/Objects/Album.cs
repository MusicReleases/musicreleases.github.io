using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class Album : IComparable
{
    public string Id { get; private set; }
    public string Name { get; private set; }
    public string ReleaseDate { get; private set; }
    public string AlbumType { get; private set; }
    public int TotalTracks { get; private set; }
    public string Uri { get; private set; }

    public string ImageUrl { get; private set; }

    public List<Image> Images { get; private set; }

    public HashSet<Artist> Artists { get; private set; }

    // TODO artists - GetArtists
    // TODO images (0), default
    public Album(SimpleAlbum simpleAlbum)
    {
        Id = simpleAlbum.Id;
        Name = simpleAlbum.Name;
        ReleaseDate = simpleAlbum.ReleaseDate;
        AlbumType = simpleAlbum.AlbumType;
        TotalTracks = simpleAlbum.TotalTracks;
        Images = simpleAlbum.Images;
        if (simpleAlbum.Images.Count > 0)
        {
            ImageUrl = simpleAlbum.Images.First().Url;
        }
        else
        {
            ImageUrl = "";
        }
        Uri = simpleAlbum.Uri;
        Artists = new();
        //Artists = Controller.GetArtists(simpleAlbum.Artists);
    }
    public Album(FullAlbum fullAlbum)
    {
        Id = fullAlbum.Id;
        Name = fullAlbum.Name;
        ReleaseDate = fullAlbum.ReleaseDate;
        AlbumType = fullAlbum.AlbumType;
        TotalTracks = fullAlbum.TotalTracks;
        Images = fullAlbum.Images;
        if (fullAlbum.Images.Count > 0)
        {
            ImageUrl = fullAlbum.Images.First().Url;
        }
        else
        {
            ImageUrl = "";
        }
        Uri = fullAlbum.Uri;
        Artists = new();
        //Artists = Controller.GetArtists(fullAlbum.Artists);
    }
    public Album(SimpleShow simpleShow)
    {
        Id = simpleShow.Id;
        Name = simpleShow.Name;
        ReleaseDate = "0";
        AlbumType = "Podcast";
        TotalTracks = 1;
        Images = simpleShow.Images;
        if (simpleShow.Images.Count > 0)
        {
            ImageUrl = simpleShow.Images.First().Url;
        }
        else
        {
            ImageUrl = "";
        }
        Uri = simpleShow.Uri;
        Artists = new()
        {
            new(id: "0", name: simpleShow.Publisher)
        };
    }


    public int CompareTo(object obj)
    {
        var other = (Album)obj;
        var lastNameComparison = -ReleaseDate.CompareTo(other.ReleaseDate);

        return (lastNameComparison != 0)
            ? lastNameComparison :
            (Id.CompareTo(other.Name));
    }
}
