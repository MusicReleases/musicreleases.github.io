namespace JakubKastner.SpotifyApi.RetryHandlers;

internal interface IAsyncSleeper
{
	Task SleepAsync(TimeSpan duration, CancellationToken cancellationToken = default);
}
