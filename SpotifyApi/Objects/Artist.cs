using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class Artist : IComparable
{
    public string Id { get; set; }
    public string Name { get; set; }

    public Artist(SimpleArtist simpleArtist)
    {
        Id = simpleArtist.Id;
        Name = simpleArtist.Name;
    }
    public Artist(FullArtist fullArtist)
    {
        Id = fullArtist.Id;
        Name = fullArtist.Name;
    }
    public Artist(string id, string name)
    {
        Id = id;
        Name = name;
    }

    public int CompareTo(object obj)
    {
        var other = (Artist)obj;
        var lastNameComparison = Name.CompareTo(other.Name);

        return (lastNameComparison != 0) ? lastNameComparison : Id.CompareTo(other.Id);
    }
}
