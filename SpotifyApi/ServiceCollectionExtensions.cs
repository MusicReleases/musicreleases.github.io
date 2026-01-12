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

		// api clients
		services.AddScoped<IApiArtistClient, ApiArtistClient>();
		services.AddScoped<IApiPlaylistClient, ApiPlaylistClient>();

		// api controllers
		services.AddScoped<IApiArtistServiceOld, ApiArtistServiceOld>();
		services.AddScoped<IApiPlaylistServiceOld, ApiPlaylistServiceOld>();
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