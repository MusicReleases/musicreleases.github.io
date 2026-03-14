using JakubKastner.MusicReleases.BackgroundTasks.Services;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.SpotifyServices;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.MusicReleases.Spotify.Artists;
using JakubKastner.MusicReleases.Spotify.Artists.Releases;
using JakubKastner.MusicReleases.Spotify.Artists.Tracks;
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

		services.AddScoped<IDbSpotifyUserService, DbSpotifyUserService>();
		services.AddScoped<IDbSpotifyUserUpdateService, DbSpotifyUserUpdateService>();
		services.AddScoped<IDbSpotifyUserSettingsService, DbSpotifyUserSettingsService>();

		services.AddScoped<IDbSpotifyUserFilterReleaseService, DbSpotifyUserFilterReleaseService>();
		services.AddScoped<IDbSpotifyUserFilterTaskService, DbSpotifyUserFilterTaskService>();

		services.AddScoped<IDbSpotifyUserArtistService, DbSpotifyUserArtistService>();
		services.AddScoped<ISpotifyArtistDbService, SpotifyArtistDbService>();
		services.AddScoped<ISpotifyArtistReleaseDbService, SpotifyArtistReleaseDbService>();

		services.AddScoped<IDbSpotifyReleaseService, DbSpotifyReleaseService>();

		services.AddScoped<IDbSpotifyUserPlaylistService, DbSpotifyUserPlaylistService>();
		services.AddScoped<IDbSpotifyPlaylistService, DbSpotifyPlaylistService>();

		services.AddScoped<IDbSpotifyTrackService, DbSpotifyTrackService>();

		services.AddScoped<ISpotifyArtistTrackDbService, SpotifyArtistTrackDbService>();

		// spotify state
		services.AddScoped<ISpotifyArtistState, SpotifyArtistState>();
		services.AddScoped<ISpotifyReleaseState, SpotifyReleaseState>();
		services.AddScoped<ISpotifyPlaylistState, SpotifyPlaylistState>();

		// spotify services
		services.AddScoped<IApiLoginService, SpotifyLoginService>();
		services.AddScoped<ISpotifyLoginService, SpotifyLoginService>();
		services.AddScoped<ISpotifyLoginStorageService, SpotifyLoginStorageService>();

		services.AddScoped<ISpotifyWorkflowService, SpotifyWorkflowService>();
		services.AddScoped<ILoadingService, LoadingService>();

		services.AddScoped<IBackgroundTaskManagerService, BackgroundTaskManagerService>();
		services.AddScoped<IBackgroundTaskFilterService, BackgroundTaskFilterService>();
		services.AddScoped<IBackgroundTaskFilterUrlService, SpotifyTaskFilterUrlService>();
		services.AddScoped<IBackgroundTaskFilterUrlSynchronizer, BackgroundTaskFilterUrlSynchronizer>();

		services.AddScoped<ISpotifyReleaseFilterService, SpotifyReleaseFilterService>();
		services.AddScoped<ISpotifyReleaseFilterUrlSynchronizer, SpotifyReleaseFilterUrlSynchronizer>();
		services.AddScoped<ISpotifyReleaseFilterUrlService, SpotifyReleaseFilterUrlService>();

		services.AddScoped<ISpotifyArtistFilterService, SpotifyArtistFilterService>();

		services.AddScoped<ISpotifyPlaylistFilterService, SpotifyPlaylistFilterService>();

		services.AddScoped<ISpotifyReleaseService, SpotifyReleaseService>();
		services.AddScoped<ISpotifyArtistService, SpotifyArtistService>();
		services.AddScoped<ISpotifyPlaylistService, SpotifyPlaylistService>();
		services.AddScoped<ISpotifyTrackService, SpotifyTrackService>();

		return services;
	}
}