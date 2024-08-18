using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistsStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleasesStore;
using JakubKastner.SpotifyApi.Objects;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Controllers.ApiControllers.SpotifyControllers;

public class SpotifyWorkflowController(IDispatcher dispatcher, IState<SpotifyPlaylistsState> stateSpotifyPlaylists, IState<SpotifyPlaylistsTracksState> stateSpotifyPlaylistsTracks, IState<SpotifyArtistsState> stateSpotifyArtists, IState<SpotifyReleasesState> stateSpotifyReleases) : ISpotifyWorkflowController
{
	private readonly IDispatcher _dispatcher = dispatcher;
	private readonly IState<SpotifyPlaylistsState> _stateSpotifyPlaylists = stateSpotifyPlaylists;
	private readonly IState<SpotifyPlaylistsTracksState> _stateSpotifyPlaylistsTracks = stateSpotifyPlaylistsTracks;
	private readonly IState<SpotifyArtistsState> _stateSpotifyArtists = stateSpotifyArtists;
	private readonly IState<SpotifyReleasesState> _stateSpotifyReleases = stateSpotifyReleases;

	public async Task StartLoadingAll(bool forceUpdate, ReleaseType releaseType)
	{
		var playlists = await StartLoadingPlaylists(forceUpdate);

		await StartLoadingArtistsWithReleases(forceUpdate, releaseType);

		if (playlists is not null)
		{
			await StartLoadingPlaylistsTracks(forceUpdate, playlists);
		}
	}


	// playlists
	public async Task StartLoadingPlaylistsWithTracks(bool forceUpdate)
	{
		var playlists = await StartLoadingPlaylists(forceUpdate);
		if (playlists is null)
		{
			return;
		}
		await StartLoadingPlaylistsTracks(forceUpdate, playlists);
	}

	private async Task<SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists>?> StartLoadingPlaylists(bool forceUpdate)
	{
		if (_stateSpotifyPlaylists.Value.LoadingAny() || _stateSpotifyPlaylistsTracks.Value.LoadingAny())
		{
			return null;
		}

		var spotifyPlaylistsAction = new SpotifyPlaylistsActionGet(forceUpdate);
		_dispatcher.Dispatch(spotifyPlaylistsAction);
		await spotifyPlaylistsAction.CompletionSource.Task;

		var playlists = _stateSpotifyPlaylists.Value.Error ? null : _stateSpotifyPlaylists.Value.List;
		return playlists;
	}

	private async Task StartLoadingPlaylistsTracks(bool forceUpdate, SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists> playlists)
	{
		if (_stateSpotifyPlaylists.Value.LoadingAny() || _stateSpotifyPlaylistsTracks.Value.LoadingAny())
		{
			return;
		}

		var spotifyPlaylistsTracksAction = new SpotifyPlaylistsTracksActionGet(forceUpdate, playlists);
		_dispatcher.Dispatch(spotifyPlaylistsTracksAction);
		await spotifyPlaylistsTracksAction.CompletionSource.Task;
	}


	// artists
	public async Task StartLoadingArtistsWithReleases(bool forceUpdate, ReleaseType releaseType)
	{
		var artists = await StartLoadingArtists(forceUpdate);
		if (artists is null)
		{
			return;
		}
		await StartLoadingArtistsReleases(forceUpdate, releaseType, artists);
	}

	private async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>?> StartLoadingArtists(bool forceUpdate)
	{
		if (_stateSpotifyArtists.Value.LoadingAny() || _stateSpotifyReleases.Value.LoadingAny())
		{
			return null;
		}

		var spotifyArtistsAction = new SpotifyArtistsActionGet(forceUpdate);
		_dispatcher.Dispatch(spotifyArtistsAction);

		await spotifyArtistsAction.CompletionSource.Task;

		var artists = _stateSpotifyArtists.Value.Error ? null : _stateSpotifyArtists.Value.List;
		return artists;
	}

	public async Task StartLoadingArtistsReleases(bool forceUpdate, ReleaseType releaseType, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists> artists)
	{
		if (_stateSpotifyArtists.Value.LoadingAny() || _stateSpotifyReleases.Value.LoadingAny())
		{
			return;
		}

		var spotifyReleasesAction = new SpotifyReleasesActionGet(releaseType, forceUpdate, artists);
		_dispatcher.Dispatch(spotifyReleasesAction);

		await spotifyReleasesAction.CompletionSource.Task;
	}
}