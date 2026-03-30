using JakubKastner.MusicReleases.Database.Spotify.Services;

namespace JakubKastner.MusicReleases.Spotify.Tasks.User;

internal interface ISpotifyUserTaskFilterDbService : ISpotifyUserScopedEntityService<BackgroundTaskFilter>
{
}