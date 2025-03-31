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
		// base services
		services.AddScoped<ILoginService, LoginService>();
		services.AddScoped<IFilterService, FilterService>();

		// indexed db services
		services.AddScoped<IDbSpotifyService, DbSpotifyService>();

		services.AddScoped<IDbSpotifyUpdateService, DbSpotifyUpdateService>();

		services.AddScoped<IDbSpotifyUserService, DbSpotifyUserService>();
		services.AddScoped<IDbSpotifyUserArtistService, DbSpotifyUserArtistService>();

		services.AddScoped<IDbSpotifyArtistService, DbSpotifyArtistService>();
		services.AddScoped<IDbSpotifyArtistReleaseService, DbSpotifyArtistReleaseService>();

		services.AddScoped<IDbSpotifyReleaseService, DbSpotifyReleaseService>();

		// spotify services
		services.AddScoped<ISpotifyFilterService, SpotifyFilterService>();
		services.AddScoped<ISpotifyWorkflowService, SpotifyWorkflowService>();

		services.AddScoped<IApiLoginService, SpotifyLoginService>();
		services.AddScoped<ISpotifyLoginService, SpotifyLoginService>();
		services.AddScoped<ISpotifyLoginStorageService, SpotifyLoginStorageService>();

		return services;
	}
}