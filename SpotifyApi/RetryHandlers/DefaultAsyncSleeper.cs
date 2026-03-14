namespace JakubKastner.SpotifyApi.RetryHandlers;

internal class DefaultAsyncSleeper : IAsyncSleeper
{
	public Task SleepAsync(TimeSpan duration, CancellationToken cancellationToken = default)
	{
		return Task.Delay(duration, cancellationToken);
	}
}
