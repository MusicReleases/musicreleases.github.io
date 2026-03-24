using JakubKastner.MusicReleases.BackgroundTasks.Enums;
using JakubKastner.MusicReleases.BackgroundTasks.Services;
using JakubKastner.MusicReleases.Database.Spotify.BaseServices;
using JakubKastner.MusicReleases.Database.Spotify.Services;
using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Spotify.Artists.User;
using JakubKastner.MusicReleases.Spotify.Base;
using JakubKastner.MusicReleases.Spotify.Playlists.User;
using JakubKastner.SpotifyApi.Artists;
using JakubKastner.SpotifyApi.Clients;

namespace JakubKastner.MusicReleases.Spotify.Artists;

internal sealed class SpotifyArtistService(ISpotifyUserClient userApi, ISpotifyArtistClient artistApi, ISpotifyReadByPayloadService<SpotifyArtist, SpotifyUserArtistPayload> artistReader, ISpotifyWriteEntityService<SpotifyArtist> artistWriter, ISpotifyUserArtistDbService userArtistDb, IDbSpotifyUserUpdateService updateDb, ISpotifyArtistState artistState, IBackgroundTaskManagerService taskManager, ILoadingService loadingService)

	: SpotifyBaseSyncService<SpotifyArtist, SpotifyUserArtistPayload>(userApi, artistReader, artistWriter, userArtistDb, updateDb, artistState, taskManager, loadingService), ISpotifyArtistService
{
	private readonly ISpotifyArtistClient _artistApi = artistApi;
	private readonly ISpotifyArtistState _artistState = artistState;

	protected override BackgroundTaskType TaskType => BackgroundTaskType.ArtistsGet;

	protected override SpotifyDbUpdateType DbUpdateType => SpotifyDbUpdateType.Artists;
	protected override string TaskTitle => "Geting artists";
	protected override string TaskDescription => "Getting followed artists";
	protected override string EntityName => "artists";
	protected override string UserLinkLabel => "user-artist";
	protected override DateTime? LastSync => _artistState.LastSync;
	protected override bool IsDataInState => _artistState.Items is not null;

	protected override SpotifyUserArtistPayload CreatePayload(SpotifyArtist model) => model.ToPayload();

	protected override async Task<IReadOnlyCollection<SpotifyArtist>> ApiLoad(CancellationToken ct) => await _artistApi.GetFollowed(ct);
}