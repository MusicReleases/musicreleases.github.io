using JakubKastner.MusicReleases.Controllers.ApiControllers;
using JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;
using JakubKastner.MusicReleases.Controllers.BaseControllers;
using JakubKastner.MusicReleases.Controllers.DatabaseControllers;

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

		// indexed db controllers
		services.AddScoped<IDatabaseController, DatabaseController>();

		services.AddScoped<IDatabaseUserController, DatabaseUserController>();
		services.AddScoped<IDatabaseUpdateController, DatabaseUpdateController>();

		services.AddScoped<IDatabaseArtistsController, DatabaseArtistsController>();
		services.AddScoped<IDatabaseReleasesController, DatabaseReleasesController>();

		// spotify controllers
		services.AddScoped<IApiLoginController, SpotifyLoginController>();
		services.AddScoped<ISpotifyLoginController, SpotifyLoginController>();
		services.AddScoped<ISpotifyLoginStorageController, SpotifyLoginStorageController>();
		services.AddScoped<ISpotifyWorkflowController, SpotifyWorkflowController>();

		return services;
	}
}