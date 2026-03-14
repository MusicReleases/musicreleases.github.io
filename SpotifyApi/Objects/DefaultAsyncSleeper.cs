namespace JakubKastner.SpotifyApi.Objects;

internal class DefaultAsyncSleeper : IAsyncSleeper
{
	public Task SleepAsync(TimeSpan duration, CancellationToken cancellationToken = default)
	{
		return Task.Delay(duration, cancellationToken);
	}
}
