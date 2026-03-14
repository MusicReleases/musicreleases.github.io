using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services;
using JakubKastner.SpotifyApi.Services.Api;
using Microsoft.Extensions.DependencyInjection;
using SpotifyAPI.Web.Http;

namespace JakubKastner.SpotifyApi;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddSpotifyApi(this IServiceCollection services)
	{
		// client and user
		services.AddScoped<ISpotifyApiClient, SpotifyApiClient>();
		services.AddScoped<ISpotifyUserStore, SpotifyUserStore>();
		services.AddScoped<ISpotifyCredentialsStore, SpotifyCredentialsStore>();

		// api clients
		services.AddScoped<IApiUserClient, ApiUserClient>();
		services.AddScoped<IApiArtistClient, ApiArtistClient>();
		services.AddScoped<IApiPlaylistClient, ApiPlaylistClient>();
		services.AddScoped<IApiReleaseClient, ApiReleaseClient>();

		// api controllers
		services.AddScoped<IApiTrackService, ApiTrackService>();
		services.AddScoped<ISpotifyApiTrackService, SpotifyApiTrackService>();

		// retry handler 
		services.AddScoped<IAsyncSleeper, DefaultAsyncSleeper>();
		services.AddScoped<IRetryHandler, SpotifyApiRetryHandler>();

		services.Configure<SpotifyRetryHandlerOptions>(options =>
		{
			options.RetryTimes = 10;
			options.RetryAfter = TimeSpan.FromMilliseconds(200);
			options.TooManyRequestsConsumesARetry = true;
		});


		return services;
	}
}