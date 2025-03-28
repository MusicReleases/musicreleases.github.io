using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Services;
using JakubKastner.SpotifyApi.Services.Api;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.Extensions.DependencyInjection;

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
		services.AddScoped<SpotifyUser>();

		// api controllers
		services.AddScoped<IApiArtistService, ApiArtistService>();
		services.AddScoped<IApiPlaylistService, ApiPlaylistService>();
		services.AddScoped<IApiReleaseService, ApiReleaseService>();
		services.AddScoped<IApiUserService, ApiUserService>();

		// controllers
		services.AddScoped<ISpotifyArtistService, SpotifyArtistService>();
		services.AddScoped<ISpotifyPlaylistService, SpotifyPlaylistService>();
		services.AddScoped<ISpotifyReleaseService, SpotifyReleaseService>();
		services.AddScoped<ISpotifyTrackService, SpotifyTrackService>();
		services.AddScoped<ISpotifyUserService, SpotifyUserService>();

		return services;
	}
}