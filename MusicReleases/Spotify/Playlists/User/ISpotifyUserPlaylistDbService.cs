using JakubKastner.MusicReleases.Database.Spotify.Services.Links;
using JakubKastner.SpotifyApi.Playlists;

namespace JakubKastner.MusicReleases.Spotify.Playlists.User;

internal interface ISpotifyUserPlaylistDbService : ISpotifyUserLinkEntityService<SpotifyUserPlaylistPayload>, ISpotifyUserLinkEntityService
{
	Task AddNew(SpotifyPlaylist playlist, string userId, CancellationToken ct);
}