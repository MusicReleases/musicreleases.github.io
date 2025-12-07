using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.SpotifyApi.Services;

public interface ISpotifyApiPlaylistService
{
	Task<SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>> GetPlaylistsTracks(SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? playlistsStorage = null, bool forceUpdate = false);
	Task<SpotifyPlaylist?> GetUserPlaylist(string playlistId, bool getTracks = false);
	Task<SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>?> GetUserPlaylists(bool onlyEditable = false, SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? existingPlaylist = null, bool forceUpdate = false);
	Task<SpotifyPlaylist> Create(string playlistName);
	Task<SpotifyPlaylist> AddTracks(SpotifyPlaylist playlist, ISet<SpotifyTrack> tracks, bool positionTop);
	Task<SpotifyPlaylist> RemoveTracks(SpotifyPlaylist playlist, ISet<SpotifyTrack> tracks);
}