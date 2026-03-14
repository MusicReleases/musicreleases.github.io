using JakubKastner.SpotifyApi.Clients;
using JakubKastner.SpotifyApi.RetryHandlers;
using JakubKastner.SpotifyApi.Services;
using JakubKastner.SpotifyApi.Services.Api;
using JakubKastner.SpotifyApi.Store;
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

		// api clients
		services.AddScoped<ISpotifyUserClient, SpotifyUserClient>();
		services.AddScoped<ISpotifyArtistClient, SpotifyArtistClient>();
		services.AddScoped<ISpotifyPlaylistClient, SpotifyPlaylistClient>();
		services.AddScoped<ISpotifyReleaseClient, SpotifyReleaseClient>();

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