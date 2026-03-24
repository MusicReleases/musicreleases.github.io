using JakubKastner.MusicReleases.Database.Spotify.Services.Links;

namespace JakubKastner.MusicReleases.Spotify.Artists.User;

internal interface ISpotifyUserArtistDbService : ISpotifyUserLinkEntityService<SpotifyUserArtistPayload>, ISpotifyUserLinkEntityService
{
}