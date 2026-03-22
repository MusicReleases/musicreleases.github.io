using JakubKastner.MusicReleases.Database.Spotify.Links;
using JakubKastner.SpotifyApi.Playlists;

namespace JakubKastner.MusicReleases.Spotify.Playlists.User;

internal interface ISpotifyUserPlaylistDbService : ISpotifyUserLinkEntityService<SpotifyUserPlaylistPayload>
{
	Task AddNew(SpotifyPlaylist playlist, string userId, CancellationToken ct);
}