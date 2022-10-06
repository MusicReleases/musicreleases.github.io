using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify.Objects;

public class Track
{
    public string Id { get; private set; }
    public string Name { get; private set; }

    public Album? Album { get; private set; }

    public HashSet<Artist> Artists { get { return _artists; } }
    private readonly HashSet<Artist> _artists;

    public string ArtistsString
    {
        get
        {
            return string.Join(" • ", _artists.Select(x => x.Name));
        }
    }

    // TODO artists - GetArtists
    public Track(SimpleTrack simpleTrack)
    {
        Id = simpleTrack.Id;
        Name = simpleTrack.Name;
        // TODO empty??
        Album = null;
        _artists = new();
        //_artists = Controller.GetArtists(simpleTrack.Artists);
    }
    public Track(FullTrack fullTrack)
    {
        Id = fullTrack.Id;
        Name = fullTrack.Name;
        Album = new(fullTrack.Album);
        _artists = new();
        //_artists = Controller.GetArtists(fullTrack.Artists);
    }
    public Track(FullEpisode fullEpisode)
    {
        Id = fullEpisode.Id;
        Name = fullEpisode.Name;
        Album = new(fullEpisode.Show);
        _artists = new()
        {
            new(id: "0", name: "podcast")
        };
    }
}
