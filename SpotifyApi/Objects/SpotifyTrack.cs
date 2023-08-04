using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Objects;

public class SpotifyTrack : IComparable
{
    public string Id { get; private set; }
    public string Name { get; private set; }

    public SpotifyAlbum? Album { get; private set; }

    public HashSet<SpotifyArtist> Artists { get { return _artists; } }
    private readonly HashSet<SpotifyArtist> _artists;

    public string ArtistsString
    {
        get
        {
            return string.Join(" • ", _artists.Select(x => x.Name));
        }
    }

    // TODO artists - GetArtists
    public SpotifyTrack(SimpleTrack simpleTrack)
    {
        Id = simpleTrack.Id;
        Name = simpleTrack.Name;
        // TODO empty??
        Album = null;
        _artists = new();
        //_artists = Controller.GetArtists(simpleTrack.Artists);
    }
    public SpotifyTrack(FullTrack fullTrack)
    {
        Id = fullTrack.Id;
        Name = fullTrack.Name;
        Album = new(fullTrack.Album);
        _artists = new();
        //_artists = Controller.GetArtists(fullTrack.Artists);
    }
    public SpotifyTrack(FullEpisode fullEpisode)
    {
        Id = fullEpisode.Id;
        Name = fullEpisode.Name;
        Album = new(fullEpisode.Show);
        _artists = new()
        {
            new(id: "0", name: "podcast")
        };
    }


    public int CompareTo(object? obj)
    {
        if (obj == null)
        {
            return -1;
        }

        var other = (SpotifyTrack)obj;
        var nameComparison = Name.CompareTo(other.Name);

        return (nameComparison != 0) ? nameComparison : Id.CompareTo(other.Id);
    }

    public override bool Equals(object? obj)
    {
        if (obj == null)
        {
            return this == null;
        }

        var other = (SpotifyTrack)obj;
        return string.Equals(Id, other.Id);
    }

    public override int GetHashCode()
    {
        if (Id == null)
        {
            return new();
        }
        return Id.GetHashCode();
    }
}
