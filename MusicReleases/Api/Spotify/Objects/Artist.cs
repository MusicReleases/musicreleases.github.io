using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify.Objects
{
    public class Artist
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
    }
}
