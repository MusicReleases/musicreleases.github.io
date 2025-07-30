using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.UiServices;

namespace JakubKastner.MusicReleases;

/// <summary>
/// Extensions for <see cref="IServiceCollection"/>
/// </summary>
public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddMusicReleases(this IServiceCollection services)
	{
		// base services
		services.AddScoped<IMobileService, MobileService>();
		services.AddScoped<ILoginService, LoginService>();

		// indexed db services
		services.AddScoped<IDbSpotifyService, DbSpotifyService>();

		services.AddScoped<IDbSpotifyUpdateService, DbSpotifyUpdateService>();
		services.AddScoped<IDbSpotifyFilterService, DbSpotifyFilterService>();

		services.AddScoped<IDbSpotifyUserService, DbSpotifyUserService>();
		services.AddScoped<IDbSpotifyUserArtistService, DbSpotifyUserArtistService>();

		services.AddScoped<IDbSpotifyArtistService, DbSpotifyArtistService>();
		services.AddScoped<IDbSpotifyArtistReleaseService, DbSpotifyArtistReleaseService>();

		services.AddScoped<IDbSpotifyReleaseService, DbSpotifyReleaseService>();

		services.AddScoped<IDbSpotifyPlaylistService, DbSpotifyPlaylistService>();
		services.AddScoped<IDbSpotifyUserPlaylistService, DbSpotifyUserPlaylistService>();

		// spotify services
		services.AddScoped<ISpotifyFilterUrlService, SpotifyFilterUrlService>();
		services.AddScoped<ISpotifyWorkflowService, SpotifyWorkflowService>();

		services.AddScoped<IApiLoginService, SpotifyLoginService>();
		services.AddScoped<ISpotifyLoginService, SpotifyLoginService>();
		services.AddScoped<ISpotifyLoginStorageService, SpotifyLoginStorageService>();

		services.AddScoped<ISpotifyFilterService, SpotifyFilterService>();
		services.AddScoped<ILoaderService, LoaderService>();

		services.AddScoped<ISpotifyReleasesService, SpotifyReleasesService>();
		services.AddScoped<ISpotifyArtistsService, SpotifyArtistsService>();
		services.AddScoped<ISpotifyPlaylistsService, SpotifyPlaylistsService>();
		services.AddScoped<ISpotifyTracksService, SpotifyTracksService>();

		return services;
	}
}