using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.SpotifyApi.Playlists;

namespace JakubKastner.MusicReleases.Spotify.Playlists.User;

internal interface ISpotifyUserPlaylistDbService : ISpotifyUserIdEntityService<SpotifyUserPlaylistPayload>
{
	Task DeleteAllForUser(string userId);
	Task AddNew(SpotifyPlaylist playlist, string userId, CancellationToken ct);
}