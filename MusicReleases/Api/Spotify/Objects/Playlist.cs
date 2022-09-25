using SpotifyAPI.Web;

namespace MusicReleases.Api.Spotify.Objects
{
    public class Playlist
    {
        public string Id { get; private set; }
        public string Name { get; private set; }
        //public bool CurrentUserOwned { get; private set; }

        public int? TotalTracks { get; private set; }

        public List<Track> Tracks { get; set; } = new();

        // TODO playlist owner - currentuserowned
        public Playlist(SimplePlaylist simplePlaylist)
        {
            Id = simplePlaylist.Id;
            Name = simplePlaylist.Name;
            TotalTracks = simplePlaylist.Tracks.Total;
            //CurrentUserOwned = IsCurrentUserPlaylist(simplePlaylist.Owner.Id);
        }
        public Playlist(FullPlaylist fullPlaylist)
        {
            // TODO null
            Id = fullPlaylist.Id ?? "";
            Name = fullPlaylist.Name ?? "";
            TotalTracks = fullPlaylist.Tracks?.Total;
            /*if (fullPlaylist.Owner == null)
            {
                CurrentUserOwned = false;
            }
            else
            {
                CurrentUserOwned = IsCurrentUserPlaylist(fullPlaylist.Owner.Id);
            }*/
        }

        /*private static bool IsCurrentUserPlaylist(string playlistOwnerId)
        {
            return playlistOwnerId == Base.User?.LoggedIn.Id;
        }*/
    }
}
