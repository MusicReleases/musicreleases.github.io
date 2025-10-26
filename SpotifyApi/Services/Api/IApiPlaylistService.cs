using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Services.Api;

internal interface IApiPlaylistService
{
	Task<ISet<SpotifyPlaylist>> GetUserPlaylistsFromApi(ISet<SpotifyPlaylist>? existingPlaylists = null);
	Task<ISet<SpotifyPlaylist>> GetPlaylistsTracksFromApi(ISet<SpotifyPlaylist> playlists, bool forceUpdate);
	Task<IList<SpotifyTrack>> GetPlaylistTracksFromApi(string playlistId);
	Task<SpotifyPlaylist> CreatePlaylistInApi(string playlistName);
}