/*using SpotifyAPI.Web.Http;

namespace JakubKastner.SpotifyApi.Base;

public class SpotifyApiRetryHandler : IRetryHandler
{
	public Task<IResponse> HandleRetry(IRequest request, IResponse response, IRetryHandler.RetryFunc retry)
	{
		// request is the sent request and response is the received response, obviously

		// don't retry:
		//return response;

		// retry once:
		var newResponse = retry(request);
		return newResponse;

		// use retry as often as you want, make sure to return a response
	}
}*/
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;
using System.Net;

namespace JakubKastner.SpotifyApi.Base;

public class SpotifyApiRetryHandler : IRetryHandler
{
	private readonly Func<TimeSpan, Task> _sleep;

	//
	// Summary:
	//     Specifies after how many miliseconds should a failed request be retried.
	public TimeSpan RetryAfter { get; set; }

	//
	// Summary:
	//     Maximum number of tries for one failed request.
	public int RetryTimes { get; set; }

	//
	// Summary:
	//     Whether a failure of type "Too Many Requests" should use up one of the allocated
	//     retry attempts.
	public bool TooManyRequestsConsumesARetry { get; set; }

	//
	// Summary:
	//     Error codes that will trigger auto-retry
	public IEnumerable<HttpStatusCode> RetryErrorCodes { get; set; }

	//
	// Summary:
	//     A simple retry handler which retries a request based on status codes with a fixed
	//     sleep interval. It also supports Retry-After headers sent by spotify. The execution
	//     will be delayed by the amount in the Retry-After header
	public SpotifyApiRetryHandler()
		: this(Task.Delay)
	{
	}

	public SpotifyApiRetryHandler(Func<TimeSpan, Task> sleep)
	{
		_sleep = sleep;
		RetryAfter = TimeSpan.FromMilliseconds(50.0);
		RetryTimes = 10;
		TooManyRequestsConsumesARetry = false;
		RetryErrorCodes = new HttpStatusCode[3]
		{
			HttpStatusCode.InternalServerError,
			HttpStatusCode.BadGateway,
			HttpStatusCode.ServiceUnavailable
		};
	}

	private static TimeSpan? ParseTooManyRetries(IResponse response)
	{
		if (response.StatusCode != HttpStatusCode.TooManyRequests)
		{
			return null;
		}

		if ((response.Headers.ContainsKey("Retry-After") && int.TryParse(response.Headers["Retry-After"], out var result)) || (response.Headers.ContainsKey("retry-after") && int.TryParse(response.Headers["retry-after"], out result)))
		{
			return TimeSpan.FromSeconds(result);
		}

		throw new APIException("429 received, but unable to parse Retry-After Header. This should not happen!");
	}

	public Task<IResponse> HandleRetry(IRequest request, IResponse response, IRetryHandler.RetryFunc retry, CancellationToken cancel = default(CancellationToken))
	{
		EnsureMe.ArgumentNotNull(response, "response");
		EnsureMe.ArgumentNotNull(retry, "retry");
		return HandleRetryInternally(request, response, retry, RetryTimes, cancel);
	}

	private async Task<IResponse> HandleRetryInternally(IRequest request, IResponse response, IRetryHandler.RetryFunc retry, int triesLeft, CancellationToken cancel)
	{
		cancel.ThrowIfCancellationRequested();
		TimeSpan? timeSpan = ParseTooManyRetries(response);
		if (timeSpan.HasValue && (!TooManyRequestsConsumesARetry || triesLeft > 0))
		{
			await _sleep(timeSpan.Value).ConfigureAwait(continueOnCapturedContext: false);
			response = await retry(request, cancel).ConfigureAwait(continueOnCapturedContext: false);
			int triesLeft2 = (TooManyRequestsConsumesARetry ? (triesLeft - 1) : triesLeft);
			return await HandleRetryInternally(request, response, retry, triesLeft2, cancel).ConfigureAwait(continueOnCapturedContext: false);
		}

		if (RetryErrorCodes.Contains(response.StatusCode) && triesLeft > 0)
		{
			await _sleep(RetryAfter).ConfigureAwait(continueOnCapturedContext: false);
			response = await retry(request, cancel).ConfigureAwait(continueOnCapturedContext: false);
			return await HandleRetryInternally(request, response, retry, triesLeft - 1, cancel).ConfigureAwait(continueOnCapturedContext: false);
		}

		return response;
	}
}