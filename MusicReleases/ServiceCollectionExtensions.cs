using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
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
		services.AddScoped<ISettingsService, SettingsService>();

		// ui services
		services.AddScoped<IIconService, IconService>();
		services.AddScoped<IDragDropService, DragDropService>();
		services.AddScoped<IMobileService, MobileService>();
		services.AddScoped<IOverflowMenuService, OverflowMenuService>();
		services.AddScoped<IPopupService, PopupService>();

		// indexed db services
		services.AddScoped<IDbSpotifyService, DbSpotifyService>();

		services.AddScoped<IDbSpotifyUpdateService, DbSpotifyUpdateService>();

		services.AddScoped<IDbSpotifyReleaseFilterService, DbSpotifyReleaseFilterService>();

		services.AddScoped<IDbSpotifyTaskFilterService, DbSpotifyTaskFilterService>();

		services.AddScoped<IDbSpotifyUserService, DbSpotifyUserService>();

		services.AddScoped<IDbSpotifyUserSettingsService, DbSpotifyUserSettingsService>();

		services.AddScoped<IDbSpotifyUserArtistService, DbSpotifyUserArtistService>();

		services.AddScoped<IDbSpotifyArtistService, DbSpotifyArtistService>();

		services.AddScoped<IDbSpotifyArtistReleaseService, DbSpotifyArtistReleaseService>();

		services.AddScoped<IDbSpotifyReleaseService, DbSpotifyReleaseService>();

		services.AddScoped<IDbSpotifyTrackService, DbSpotifyTrackService>();

		services.AddScoped<IDbSpotifyArtistTrackService, DbSpotifyArtistTrackService>();

		services.AddScoped<IDbSpotifyPlaylistService, DbSpotifyPlaylistService>();

		services.AddScoped<IDbSpotifyUserPlaylistService, DbSpotifyUserPlaylistService>();

		// spotify state
		services.AddScoped<ISpotifyArtistState, SpotifyArtistState>();
		services.AddScoped<ISpotifyPlaylistState, SpotifyPlaylistState>();
		services.AddScoped<ISpotifyReleaseState, SpotifyReleaseState>();

		// spotify services
		services.AddScoped<ISpotifyTaskManagerService, SpotifyTaskManagerService>();
		services.AddScoped<ISpotifyTaskFilterService, SpotifyTaskFilterService>();
		services.AddScoped<ISpotifyTaskFilterUrlService, SpotifyTaskFilterUrlService>();
		services.AddScoped<ISpotifyTaskFilterUrlSynchronizer, SpotifyTaskFilterUrlSynchronizer>();

		services.AddScoped<ISpotifyReleaseFilterService, SpotifyReleaseFilterService>();
		services.AddScoped<ISpotifyReleaseFilterUrlSynchronizer, SpotifyReleaseFilterUrlSynchronizer>();
		services.AddScoped<ISpotifyReleaseFilterUrlService, SpotifyReleaseFilterUrlService>();

		services.AddScoped<ISpotifyWorkflowService, SpotifyWorkflowService>();

		services.AddScoped<IApiLoginService, SpotifyLoginService>();
		services.AddScoped<ISpotifyLoginService, SpotifyLoginService>();
		services.AddScoped<ISpotifyLoginStorageService, SpotifyLoginStorageService>();

		services.AddScoped<ISpotifyFilterPlaylistService, SpotifyFilterPlaylistService>();
		services.AddScoped<ILoaderService, LoaderService>();

		services.AddScoped<ISpotifyReleaseService, SpotifyReleaseService>();
		services.AddScoped<ISpotifyArtistService, SpotifyArtistService>();
		services.AddScoped<ISpotifyPlaylistService, SpotifyPlaylistService>();
		services.AddScoped<ISpotifyTrackService, SpotifyTrackService>();

		return services;
	}
}