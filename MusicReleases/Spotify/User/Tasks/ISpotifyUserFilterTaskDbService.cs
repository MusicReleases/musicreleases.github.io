using JakubKastner.MusicReleases.Database.Spotify.BaseServices;
using JakubKastner.MusicReleases.Objects.Spotify;

namespace JakubKastner.MusicReleases.Spotify.User.Tasks;

internal interface ISpotifyUserFilterTaskDbService : ISpotifyUserScopedEntityService<BackgroundTaskFilter>
{
}