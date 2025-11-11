using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public interface ISpotifyPlaylistsService
{
	SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>? Playlists { get; }

	event Action? OnPlaylistsDataChanged;

	Task Get(bool forceUpdate);
	Task Create(string playlistName);
	Task<SpotifyPlaylist> AddTracks(SpotifyPlaylist playlist, ISet<SpotifyTrack> tracks, bool positionTop);
	Task<SpotifyPlaylist> RemoveTracks(SpotifyPlaylist playlist, SortedSet<SpotifyTrack> tracks);
}