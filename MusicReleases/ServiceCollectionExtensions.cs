using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.MusicReleases.State.Spotify;

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

		// ui services
		services.AddScoped<IIconService, IconService>();
		services.AddScoped<IDragDropService, DragDropService>();
		services.AddScoped<IMobileService, MobileService>();
		services.AddScoped<IOverflowMenuService, OverflowMenuService>();
		services.AddScoped<IPopupService, PopupService>();

		// indexed db services
		services.AddScoped<IDbSpotifyService, DbSpotifyService>();
		services.AddScoped<IDbSpotifyServiceOld, DbSpotifyServiceOld>();

		services.AddScoped<IDbSpotifyUpdateService, DbSpotifyUpdateService>();
		services.AddScoped<IDbSpotifyUpdateServiceOld, DbSpotifyUpdateServiceOld>();
		services.AddScoped<IDbSpotifyFilterService, DbSpotifyFilterService>();

		services.AddScoped<IDbSpotifyUserService, DbSpotifyUserService>();
		services.AddScoped<IDbSpotifyUserArtistService, DbSpotifyUserArtistService>();
		services.AddScoped<IDbSpotifyUserArtistServiceOld, DbSpotifyUserArtistServiceOld>();

		services.AddScoped<IDbSpotifyArtistService, DbSpotifyArtistService>();
		services.AddScoped<IDbSpotifyArtistReleaseService, DbSpotifyArtistReleaseService>();

		services.AddScoped<IDbSpotifyReleaseService, DbSpotifyReleaseService>();

		services.AddScoped<IDbSpotifyPlaylistService, DbSpotifyPlaylistService>();
		services.AddScoped<IDbSpotifyUserPlaylistService, DbSpotifyUserPlaylistService>();
		services.AddScoped<IDbSpotifyPlaylistServiceOld, DbSpotifyPlaylistServiceOld>();
		services.AddScoped<IDbSpotifyUserPlaylistServiceOld, DbSpotifyUserPlaylistServiceOld>();

		// spotify state
		services.AddScoped<ISpotifyArtistState, SpotifyArtistState>();
		services.AddScoped<ISpotifyPlaylistState, SpotifyPlaylistState>();

		// spotify services
		services.AddScoped<ISpotifyTaskManagerService, SpotifyTaskManagerService>();
		services.AddScoped<ISpotifyTaskFilterService, SpotifyTaskFilterService>();
		services.AddScoped<ISpotifyTaskFilterUrlService, SpotifyTaskFilterUrlService>();
		services.AddScoped<ISpotifyTaskFilterUrlSynchronizer, SpotifyTaskFilterUrlSynchronizer>();

		services.AddScoped<ISpotifyFilterUrlService, SpotifyFilterUrlService>();
		services.AddScoped<ISpotifyWorkflowService, SpotifyWorkflowService>();

		services.AddScoped<IApiLoginService, SpotifyLoginService>();
		services.AddScoped<ISpotifyLoginService, SpotifyLoginService>();
		services.AddScoped<ISpotifyLoginStorageService, SpotifyLoginStorageService>();

		services.AddScoped<ISpotifyFilterService, SpotifyFilterService>();
		services.AddScoped<ISpotifyFilterPlaylistService, SpotifyFilterPlaylistService>();
		services.AddScoped<ILoaderService, LoaderService>();

		services.AddScoped<ISpotifyReleasesService, SpotifyReleasesService>();
		services.AddScoped<ISpotifyArtistService, SpotifyArtistService>();
		services.AddScoped<ISpotifyArtistsServiceOld, SpotifyArtistsServiceOld>();
		services.AddScoped<ISpotifyPlaylistService, SpotifyPlaylistService>();
		services.AddScoped<ISpotifyPlaylistsServiceOld, SpotifyPlaylistsServiceOld>();
		services.AddScoped<ISpotifyTracksService, SpotifyTracksService>();

		return services;
	}
}