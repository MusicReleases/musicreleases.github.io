using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services;
using JakubKastner.SpotifyApi.Services.Api;
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
		services.AddScoped<IApiArtistServiceOld, ApiArtistServiceOld>();
		services.AddScoped<IApiPlaylistService, ApiPlaylistService>();
		services.AddScoped<IApiReleaseService, ApiReleaseService>();
		services.AddScoped<IApiUserService, ApiUserService>();
		services.AddScoped<IApiTrackService, ApiTrackService>();

		// controllers
		services.AddScoped<ISpotifyApiArtistService, SpotifyApiArtistService>();
		services.AddScoped<ISpotifyApiPlaylistService, SpotifyApiPlaylistService>();
		services.AddScoped<ISpotifyApiReleaseService, SpotifyApiReleaseService>();
		services.AddScoped<ISpotifyApiTrackService, SpotifyApiTrackService>();
		services.AddScoped<ISpotifyApiUserService, SpotifyApiUserService>();

		return services;
	}
}