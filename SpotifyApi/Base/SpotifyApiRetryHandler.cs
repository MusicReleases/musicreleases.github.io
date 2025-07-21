using SpotifyAPI.Web.Http;
using System.Net;

namespace JakubKastner.SpotifyApi.Base;

// https://github.com/JohnnyCrazy/SpotifyAPI-NET/blob/master/SpotifyAPI.Web/RetryHandlers/SimpleRetryHandler.cs
public class SpotifyApiRetryHandler : IRetryHandler
{
	private readonly Func<TimeSpan, CancellationToken, Task> _sleep;

	/// <summary>
	///     Specifies after how many milliseconds should a failed request be retried.
	/// </summary>
	public TimeSpan RetryAfter { get; set; }

	/// <summary>
	///     Maximum number of tries for one failed request.
	/// </summary>
	public int RetryTimes { get; set; }

	/// <summary>
	///     Whether a failure of type "Too Many Requests" should use up one of the allocated retry attempts.
	/// </summary>
	public bool TooManyRequestsConsumesARetry { get; set; }

	/// <summary>
	///     Error codes that will trigger auto-retry
	/// </summary>
	public IEnumerable<HttpStatusCode> RetryErrorCodes { get; set; }

	/// <summary>
	///   A simple retry handler which retries a request based on status codes with a fixed sleep interval.
	///   It also supports Retry-After headers sent by spotify. The execution will be delayed by the amount in
	///   the Retry-After header
	/// </summary>
	/// <returns></returns>
	public SpotifyApiRetryHandler() : this(sleepWithCancel: Task.Delay)
	{

	}

	public SpotifyApiRetryHandler(Func<TimeSpan, Task> sleep) : this((t, _) => sleep(t))
	{

	}

	public SpotifyApiRetryHandler(Func<TimeSpan, CancellationToken, Task> sleepWithCancel)
	{
		_sleep = sleepWithCancel;
		RetryAfter = TimeSpan.FromMilliseconds(200);
		RetryTimes = 10;
		TooManyRequestsConsumesARetry = true;
		RetryErrorCodes = [HttpStatusCode.InternalServerError, HttpStatusCode.BadGateway, HttpStatusCode.ServiceUnavailable];
	}
	private static TimeSpan? ParseTooManyRetries(IResponse response)
	{
		if (response.StatusCode != (HttpStatusCode)429)
		{
			return null;
		}
		if (
		  (response.Headers.ContainsKey("Retry-After") && int.TryParse(response.Headers["Retry-After"], out int secondsToWait))
		  || (response.Headers.ContainsKey("retry-after") && int.TryParse(response.Headers["retry-after"], out secondsToWait)))
		{
			return TimeSpan.FromSeconds(secondsToWait);
		}
		// wait 0,5 minute
		return TimeSpan.FromSeconds(30);
		//throw new APIException("429 received, but unable to parse Retry-After Header (" + response.Headers["Retry-After"] + " | " + response.Headers["retry-after"] + "). This should not happen!");
	}

	public Task<IResponse> HandleRetry(IRequest request, IResponse response, IRetryHandler.RetryFunc retry, CancellationToken cancel = default)
	{
		EnsureMe.ArgumentNotNull(response, nameof(response));
		EnsureMe.ArgumentNotNull(retry, nameof(retry));

		return HandleRetryInternally(request, response, retry, RetryTimes, cancel);
	}

	private async Task<IResponse> HandleRetryInternally(IRequest request, IResponse response, IRetryHandler.RetryFunc retry, int triesLeft, CancellationToken cancel)
	{
		cancel.ThrowIfCancellationRequested();

		var secondsToWait = ParseTooManyRetries(response);
		if (secondsToWait is not null && (!TooManyRequestsConsumesARetry || triesLeft > 0))
		{
			await _sleep(secondsToWait.Value, cancel).ConfigureAwait(false);
			response = await retry(request, cancel).ConfigureAwait(false);
			var newTriesLeft = TooManyRequestsConsumesARetry ? triesLeft - 1 : triesLeft;
			return await HandleRetryInternally(request, response, retry, newTriesLeft, cancel).ConfigureAwait(false);
		}

		while (RetryErrorCodes.Contains(response.StatusCode) && triesLeft > 0)
		{
			await _sleep(RetryAfter, cancel).ConfigureAwait(false);
			response = await retry(request, cancel).ConfigureAwait(false);
			return await HandleRetryInternally(request, response, retry, triesLeft - 1, cancel).ConfigureAwait(false);
		}

		return response;
	}
}