using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Services.Api;

internal interface IApiPlaylistService
{
	Task<ISet<SpotifyPlaylist>> GetUserPlaylistsFromApi(ISet<SpotifyPlaylist>? existingPlaylists = null);
	Task<ISet<SpotifyPlaylist>> GetPlaylistsTracksFromApi(ISet<SpotifyPlaylist> playlists, bool forceUpdate);
	Task<IList<SpotifyTrack>> GetPlaylistTracksFromApi(string playlistId);
	Task<SpotifyPlaylist> CreatePlaylistInApi(string playlistName);
	Task<string> AddTracksInApi(string playlistId, ISet<SpotifyTrack> tracks, bool positionTop);
	Task<string> RemoveTracksInApi(string id, ISet<SpotifyTrack> tracks);
}