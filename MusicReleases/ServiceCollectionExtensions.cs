using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;

namespace JakubKastner.MusicReleases;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMusicReleases(this IServiceCollection services)
	{
		// base controllers
		services.AddScoped<ILoginService, LoginService>();
		services.AddScoped<IFilterService, FilterService>();

		// indexed db controllers
		services.AddScoped<IDbSpotifyService, DbSpotifyService>();

		services.AddScoped<IDbSpotifyUpdateService, DbSpotifyUpdateService>();

		services.AddScoped<IDbSpotifyUserService, DbSpotifyUserService>();
		services.AddScoped<IDbSpotifyUserArtistService, DbSpotifyUserArtistService>();

		services.AddScoped<IDbSpotifyArtistService, DbSpotifyArtistService>();
		services.AddScoped<IDbSpotifyArtistReleaseService, DbSpotifyArtistReleaseService>();

		services.AddScoped<IDbSpotifyReleaseService, DbSpotifyReleaseService>();

		// spotify controllers
		services.AddScoped<IApiLoginService, SpotifyLoginService>();
		services.AddScoped<ISpotifyLoginService, SpotifyLoginService>();
		services.AddScoped<ISpotifyLoginStorageService, SpotifyLoginStorageService>();
		services.AddScoped<ISpotifyWorkflowService, SpotifyWorkflowService>();

		return services;
	}
}