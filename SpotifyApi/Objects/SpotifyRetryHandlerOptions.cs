using System.Net;

namespace JakubKastner.SpotifyApi.Objects;

internal class SpotifyRetryHandlerOptions
{
	/// <summary>
	///     Specifies after how many milliseconds should a failed request be retried.
	/// </summary>
	public TimeSpan RetryAfter { get; set; } = TimeSpan.FromMilliseconds(200);

	/// <summary>
	///     Maximum number of tries for one failed request.
	/// </summary>
	public int RetryTimes { get; set; } = 10;

	/// <summary>
	///     Whether a failure of type "Too Many Requests" should use up one of the allocated retry attempts.
	/// </summary>
	public bool TooManyRequestsConsumesARetry { get; set; } = true;

	/// <summary>
	///     Error codes that will trigger auto-retry
	/// </summary>
	public IEnumerable<HttpStatusCode> RetryErrorCodes { get; set; } = [HttpStatusCode.InternalServerError, HttpStatusCode.BadGateway, HttpStatusCode.ServiceUnavailable];
}
