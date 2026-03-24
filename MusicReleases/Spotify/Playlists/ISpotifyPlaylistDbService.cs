namespace JakubKastner.MusicReleases.Spotify.Playlists;

internal interface ISpotifyPlaylistDbService
{
	Task UpdateSnapshot(string playlistId, string newSnapshotId, CancellationToken ct);
}