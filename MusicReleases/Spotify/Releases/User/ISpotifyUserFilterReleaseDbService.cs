using JakubKastner.MusicReleases.Database.Spotify.BaseServices;
using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Spotify.Releases.User;

internal interface ISpotifyUserFilterReleaseDbService : ISpotifyUserScopedEntityService<SpotifyReleaseFilter>
{
}