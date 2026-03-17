using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Spotify.Playlists.User;
using JakubKastner.SpotifyApi.Playlists;

namespace JakubKastner.MusicReleases.Spotify.Playlists;

internal interface ISpotifyPlaylistDbService : ISpotifyEntityService<SpotifyPlaylist, SpotifyUserPlaylistPayload>
{
	Task UpdateSnapshot(string playlistId, string newSnapshotId, CancellationToken ct);
}