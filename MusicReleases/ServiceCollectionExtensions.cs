using JakubKastner.MusicReleases.Database.Spotify;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Database.Spotify.Services.Links;
using JakubKastner.MusicReleases.Services.ApiServices;
using JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.UiServices;
using JakubKastner.MusicReleases.Spotify;
using JakubKastner.MusicReleases.Spotify.Artists;
using JakubKastner.MusicReleases.Spotify.Artists.User;
using JakubKastner.MusicReleases.Spotify.Playlists;
using JakubKastner.MusicReleases.Spotify.Playlists.User;
using JakubKastner.MusicReleases.Spotify.Releases;
using JakubKastner.MusicReleases.Spotify.Releases.Artists;
using JakubKastner.MusicReleases.Spotify.Releases.Tracks;
using JakubKastner.MusicReleases.Spotify.Releases.User;
using JakubKastner.MusicReleases.Spotify.Settings;
using JakubKastner.MusicReleases.Spotify.Settings.User;
using JakubKastner.MusicReleases.Spotify.Tasks;
using JakubKastner.MusicReleases.Spotify.Tasks.User;
using JakubKastner.MusicReleases.Spotify.Tracks;
using JakubKastner.MusicReleases.Spotify.User;
using JakubKastner.MusicReleases.Spotify.User.Update;
using JakubKastner.SpotifyApi.Artists;
using JakubKastner.SpotifyApi.Playlists;

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
		services.AddScoped<ISpotifySettingsService, SpotifySettingsService>();

		// ui services
		services.AddScoped<IIconService, IconService>();
		services.AddScoped<IDragDropService, DragDropService>();
		services.AddScoped<IMobileService, MobileService>();
		services.AddScoped<IOverflowMenuService, OverflowMenuService>();
		services.AddScoped<IPopupService, PopupService>();

		// indexed db services
		services.AddScoped<IDbSpotifyService, DbSpotifyService>();


		services.AddAsSpotifyUserScopedService<ISpotifyUserSettingsDbService, SpotifySettings>();
		services.AddAsSpotifyUserScopedService<ISpotifyUserTaskFilterDbService, SpotifyTaskFilter>();


		services.AddScoped<ISpotifyUserDbService, SpotifyUserDbService>();
		services.AddScoped<ISpotifyUserUpdateDbService, SpotifyUserUpdateDbService>();
		//services.AddAsSpotifyService<IDbSpotifyUserUpdateService>();
		services.AddScoped<ISpotifyUserSettingsDbService, SpotifyUserSettingsDbService>();
		//services.AddAsSpotifyService<IDbSpotifyUserSettingsService>();

		services.AddScoped<ISpotifyUserReleaseFilterDbService, SpotifyUserReleaseFilterDbService>();
		services.AddAsSpotifyUserScopedService<ISpotifyUserReleaseFilterDbService, SpotifyReleaseFilter>();
		//services.AddAsSpotifyService<IDbSpotifyUserFilterReleaseService>();
		services.AddScoped<ISpotifyUserTaskFilterDbService, SpotifyUserTaskFilterDbService>();
		//services.AddAsSpotifyService<IDbSpotifyUserFilterTaskService>();

		services.AddScoped<ISpotifyUserArtistDbService, SpotifyUserArtistDbService>();
		services.AddAsSpotifyUserLinkService<ISpotifyUserArtistDbService, SpotifyUserArtistPayload>();

		services.AddScoped<ISpotifyArtistDbService, SpotifyArtistDbService>();
		services.AddScoped<ISpotifyArtistReleaseDbService, SpotifyArtistReleaseDbService>();

		services.AddScoped<ISpotifyReleaseDbService, SpotifyReleaseDbService>();

		services.AddScoped<ISpotifyUserPlaylistDbService, SpotifyUserPlaylistDbService>();
		services.AddAsSpotifyUserLinkService<ISpotifyUserPlaylistDbService, SpotifyUserPlaylistPayload>();
		services.AddScoped<ISpotifyPlaylistDbService, SpotifyPlaylistDbService>();

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
		services.AddScoped<IBackgroundTaskManagerService2, BackgroundTaskManagerService2>();
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

		services.AddScoped<ISpotifyReadByPayloadService<SpotifyPlaylist, SpotifyUserPlaylistPayload>, SpotifyPlaylistDbService>();
		services.AddScoped<ISpotifyWriteEntityService<SpotifyPlaylist>, SpotifyPlaylistDbService>();

		services.AddScoped<ISpotifyReadByPayloadService<SpotifyArtist, SpotifyUserArtistPayload>, SpotifyArtistDbService>();
		services.AddScoped<ISpotifyWriteEntityService<SpotifyArtist>, SpotifyArtistDbService>();

		return services;
	}

	private static void AddAsSpotifyUserLinkService<TInterface, TPayload>(this IServiceCollection services)
		where TInterface : class, ISpotifyUserLinkEntityService<TPayload>, ISpotifyUserLinkEntityService
		where TPayload : ISpotifyPayload
	{
		services.AddScoped<ISpotifyUserLinkEntityService<TPayload>>(sp => sp.GetRequiredService<TInterface>());
		services.AddScoped<ISpotifyUserLinkEntityService>(sp => sp.GetRequiredService<TInterface>());
	}

	private static void AddAsSpotifyUserScopedService<TInterface, TModel>(this IServiceCollection services)
		where TInterface : class, ISpotifyUserScopedEntityService<TModel>, ISpotifyUserScopedEntityService
		where TModel : class
	{
		services.AddScoped<ISpotifyUserScopedEntityService<TModel>>(sp => sp.GetRequiredService<TInterface>());

		services.AddScoped<ISpotifyUserScopedEntityService>(sp => sp.GetRequiredService<TInterface>());
	}
}