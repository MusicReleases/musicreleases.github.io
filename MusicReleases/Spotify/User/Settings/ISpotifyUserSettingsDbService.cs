using JakubKastner.MusicReleases.Database.Spotify.BaseServices;
using JakubKastner.MusicReleases.Objects.User;

namespace JakubKastner.MusicReleases.Spotify.User.Settings;

internal interface ISpotifyUserSettingsDbService : ISpotifyUserScopedEntityService<UserSettings>
{
}