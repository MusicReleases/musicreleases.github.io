using JakubKastner.MusicReleases.Controllers.ApiControllers;
using JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;
using JakubKastner.MusicReleases.Controllers.BaseControllers;
using JakubKastner.MusicReleases.Controllers.DatabaseControllers;
using JakubKastner.MusicReleases.Controllers.DatabaseControllers.SpotifyControllers;

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
		services.AddScoped<IDatabaseSpotifyController, DatabaseSpotifyController>();

		services.AddScoped<IDatabaseSpotifyUpdateController, DatabaseSpotifyUpdateController>();

		services.AddScoped<IDatabaseSpotifyUserController, DatabaseSpotifyUserController>();
		services.AddScoped<IDatabaseSpotifyUserArtistController, DatabaseSpotifyUserArtistController>();

		services.AddScoped<IDatabaseSpotifyArtistController, DatabaseSpotifyArtistController>();
		services.AddScoped<IDatabaseSpotifyArtistReleaseController, DatabaseSpotifyArtistReleaseController>();

		services.AddScoped<IDatabaseSpotifyReleaseController, DatabaseSpotifyReleaseController>();

		// spotify controllers
		services.AddScoped<IApiLoginController, SpotifyLoginController>();
		services.AddScoped<ISpotifyLoginController, SpotifyLoginController>();
		services.AddScoped<ISpotifyLoginStorageController, SpotifyLoginStorageController>();
		services.AddScoped<ISpotifyWorkflowController, SpotifyWorkflowController>();

		return services;
	}
}