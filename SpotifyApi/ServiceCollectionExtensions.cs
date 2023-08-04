using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Controllers;
using JakubKastner.SpotifyApi.Controllers.Api;
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
		services.AddScoped<IControllerApiArtist, ControllerApiArtist>();
		services.AddScoped<IControllerApiPlaylist, ControllerApiPlaylist>();
		services.AddScoped<IControllerApiRelease, ControllerApiRelease>();
		services.AddScoped<IControllerApiTrack, ControllerApiTrack>();
		services.AddScoped<IControllerApiUser, ControllerApiUser>();

		// controllers
		services.AddScoped<ISpotifyControllerArtist, SpotifyControllerArtist>();
		services.AddScoped<ISpotifyControllerPlaylist, SpotifyControllerPlaylist>();
		services.AddScoped<ISpotifyControllerRelease, SpotifyControllerRelease>();
		services.AddScoped<ISpotifyControllerTrack, SpotifyControllerTrack>();
		services.AddScoped<ISpotifyControllerUser, SpotifyControllerUser>();

		return services;
	}
}