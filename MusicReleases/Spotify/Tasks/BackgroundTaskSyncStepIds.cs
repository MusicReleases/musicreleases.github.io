namespace JakubKastner.MusicReleases.Spotify.Tasks;

internal readonly record struct BackgroundTaskSyncStepIds(Guid DbLoad, Guid ApiLoad, Guid DbSave);