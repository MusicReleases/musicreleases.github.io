namespace JakubKastner.SpotifyApi.Objects;

internal interface IAsyncSleeper
{
	Task SleepAsync(TimeSpan duration, CancellationToken cancellationToken = default);
}
