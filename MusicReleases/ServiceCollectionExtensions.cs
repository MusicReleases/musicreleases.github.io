using JakubKastner.MusicReleases.Controllers.ApiControllers;
using JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;
using JakubKastner.MusicReleases.Controllers.BaseControllers;

namespace JakubKastner.MusicReleases;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMusicReleases(this IServiceCollection services)
	{
		// base controllers
		services.AddScoped<ILoginController, LoginController>();

		// spotify controllers
		services.AddScoped<IApiLoginController, SpotifyLoginController>();
		services.AddScoped<ISpotifyLoginController, SpotifyLoginController>();
		services.AddScoped<ISpotifyLoginStorageController, SpotifyLoginStorageController>();
		services.AddScoped<ISpotifyWorkflowController, SpotifyWorkflowController>();

		return services;
	}
}