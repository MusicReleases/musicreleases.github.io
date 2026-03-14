using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Store;
using Microsoft.Extensions.Options;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Http;
using System.Net;

namespace JakubKastner.SpotifyApi.RetryHandlers;

/// <summary>
///   A simple retry handler which retries a request based on status codes with a fixed sleep interval.
///   It also supports Retry-After headers sent by spotify. The execution will be delayed by the amount in
///   the Retry-After header
/// </summary>
/// <returns></returns>
internal class SpotifyApiRetryHandler(ISpotifyClientStore apiClient, ISpotifyUserStore userStore, SpotifyConfig spotifyConfig, IAsyncSleeper sleeper, IOptions<SpotifyRetryHandlerOptions> options) : IRetryHandler
{
	private readonly ISpotifyClientStore _apiClient = apiClient;
	private readonly ISpotifyUserStore _userStore = userStore;
	private readonly SpotifyConfig _spotifyConfig = spotifyConfig;
	private readonly IAsyncSleeper _sleeper = sleeper;
	private readonly SpotifyRetryHandlerOptions _options = options.Value;

	public Task<IResponse> HandleRetry(IRequest request, IResponse response, IRetryHandler.RetryFunc retry, CancellationToken cancel = default)
	{
		return HandleRetryInternally(request, response, retry, _options.RetryTimes, cancel);
	}

	private async Task<IResponse> HandleRetryInternally(IRequest request, IResponse response, IRetryHandler.RetryFunc retry, int triesLeft, CancellationToken cancel)
	{
		cancel.ThrowIfCancellationRequested();

		// 429 Too Many Requests
		var secondsToWait = ParseTooManyRetries(response);
		if (secondsToWait is not null && (!_options.TooManyRequestsConsumesARetry || triesLeft > 0))
		{
			await _sleeper.SleepAsync(secondsToWait.Value, cancel);
			var newResponse = await retry(request, cancel);
			var newTriesLeft = _options.TooManyRequestsConsumesARetry ? triesLeft - 1 : triesLeft;
			return await HandleRetryInternally(request, newResponse, retry, newTriesLeft, cancel);
		}

		// 5xx
		if (_options.RetryErrorCodes.Contains(response.StatusCode) && triesLeft > 0)
		{
			await _sleeper.SleepAsync(_options.RetryAfter, cancel);
			var newResponse = await retry(request, cancel);
			return await HandleRetryInternally(request, newResponse, retry, triesLeft - 1, cancel);
		}

		// 401 Unauthorized – refresh + retry
		if (response.StatusCode == HttpStatusCode.Unauthorized && triesLeft > 0)
		{
			var accessToken = await RefreshTokenAsync();
			request.Headers["Authorization"] = $"Bearer {accessToken}";
			var newResponse = await retry(request, cancel);
			return await HandleRetryInternally(request, newResponse, retry, triesLeft - 1, cancel);
		}

		return response;
	}

	private async Task<string> RefreshTokenAsync()
	{
		var oldRefreshToken = _userStore.GetRefreshTokenRequired();

		var refreshRequest = new PKCETokenRefreshRequest(_spotifyConfig.ClientId, oldRefreshToken);
		var tokenResponse = await new OAuthClient().RequestToken(refreshRequest);

		_userStore.SetRefreshToken(tokenResponse.RefreshToken);

		var config = SpotifyClientConfig.CreateDefault(tokenResponse.AccessToken).WithRetryHandler(this);
		var spotifyClient = new SpotifyAPI.Web.SpotifyClient(config);
		_apiClient.SetClient(spotifyClient);

		return tokenResponse.AccessToken;
	}

	private static TimeSpan? ParseTooManyRetries(IResponse response)
	{
		if (response.StatusCode != (HttpStatusCode)429)
		{
			return null;
		}

		if ((response.Headers.ContainsKey("Retry-After") && int.TryParse(response.Headers["Retry-After"], out int secondsToWait))
			|| (response.Headers.ContainsKey("retry-after") && int.TryParse(response.Headers["retry-after"], out secondsToWait)))
		{
			return TimeSpan.FromSeconds(secondsToWait);
		}

		// wait 0,5 minute
		return TimeSpan.FromSeconds(30);
		//throw new APIException("429 received, but unable to parse Retry-After Header (" + response.Headers["Retry-After"] + " | " + response.Headers["retry-after"] + "). This should not happen!");
	}
}
