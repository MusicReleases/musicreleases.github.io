using JakubKastner.MusicReleases.Database.Spotify.IdEntities;
using JakubKastner.MusicReleases.Spotify.Playlists.User;
using JakubKastner.SpotifyApi.Playlists;

namespace JakubKastner.MusicReleases.Spotify.Playlists;

internal interface ISpotifyPlaylistDbService : ISpotifyIdEntityService<SpotifyPlaylist, SpotifyUserPlaylistPayload>
{
	Task UpdateSnapshot(string playlistId, string newSnapshotId, CancellationToken ct);
}