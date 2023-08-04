using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerPlaylist
{
    private readonly ControllerApiPlaylist _controllerApiPlaylist;
    private readonly ControllerApiTrack _controllerApiTrack;
    private readonly SpotifyUser _user;

    public SpotifyControllerPlaylist(ControllerApiPlaylist controllerApiPlaylist, ControllerApiTrack controllerApiTrack, SpotifyUser user)
    {
        _controllerApiPlaylist = controllerApiPlaylist;
        _user = user;
        _controllerApiTrack = controllerApiTrack;
    }

    // get list of user playlists
    public async Task<SortedSet<SpotifyPlaylist>> GetUserPlaylists(bool onlyEditable = false)
    {
        var playlists = _user.Playlists ??= await _controllerApiPlaylist.GetUserPlaylistsFromApi();

        if (!onlyEditable)
        {
            return new(playlists);
        }

        // TODO settings
        return new(playlists.Where(playlist => playlist.CurrentUserOwned == true && playlist.Collaborative == false));
    }

    // get user playlist (with tracks)
    public async Task<SpotifyPlaylist?> GetUserPlaylist(string playlistId, bool getTracks = false)
    {
        if (await GetUserPlaylists() == null)
        {
            // TODO getalluserplaylist() is not nullable
            return null;
        }

        var playlist = _user.Playlists!.FirstOrDefault(x => x.Id == playlistId);

        if (getTracks)
        {
            var tracks = await _controllerApiTrack.GetPlaylistTracksFromApi(playlistId);
        }

        return playlist;
    }
}
