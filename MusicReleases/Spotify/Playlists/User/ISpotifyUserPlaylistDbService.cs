using JakubKastner.MusicReleases.Database.Spotify;

namespace JakubKastner.MusicReleases.Spotify.Playlists.User;

internal interface ISpotifyUserPlaylistDbService : ISpotifyUserRelationService<SpotifyUserPlaylistPayload>
{
	Task DeleteAllForUser(string userId);
	Task AddNew(string playlistId, string userId, CancellationToken ct);
}