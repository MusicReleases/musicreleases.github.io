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

	public async Task StartLoadingAll(bool forceUpdate, ReleaseType releasesType)
	{
		var playlists = await StartLoadingPlaylists(forceUpdate);
		await StartLoadingArtistsWithReleases(forceUpdate, releasesType);
		if (playlists is null)
		{
			return;
		}
		await StartLoadingPlaylistsTracks(forceUpdate, playlists);
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

	private async Task<SpotifyUserList<SpotifyPlaylist>?> StartLoadingPlaylists(bool forceUpdate)
	{
		var spotifyPlaylistsAction = new SpotifyPlaylistsActionGet(forceUpdate);
		_dispatcher.Dispatch(spotifyPlaylistsAction);
		await spotifyPlaylistsAction.CompletionSource.Task;
		var playlists = _stateSpotifyPlaylists.Value.Error ? null : _stateSpotifyPlaylists.Value.List;
		return playlists;
	}

	private async Task StartLoadingPlaylistsTracks(bool forceUpdate, SpotifyUserList<SpotifyPlaylist> playlists)
	{
		var spotifyPlaylistsTracksAction = new SpotifyPlaylistsTracksActionGet(forceUpdate, playlists);
		_dispatcher.Dispatch(spotifyPlaylistsTracksAction);
		await spotifyPlaylistsTracksAction.CompletionSource.Task;
	}


	// artists
	public async Task StartLoadingArtistsWithReleases(bool forceUpdate, ReleaseType releasesType)
	{
		await StartLoadingArtists(forceUpdate);
		//await StartLoadingArtistsReleases(forceUpdate, releasesType);
	}

	private async Task StartLoadingArtists(bool forceUpdate)
	{
		var spotifyArtistsAction = new SpotifyArtistsActionGet(forceUpdate);
		_dispatcher.Dispatch(spotifyArtistsAction);
		//await spotifyArtistsAction.CompletionSource.Task;
	}

	public async Task StartLoadingArtistsReleases(bool forceUpdate, ReleaseType releasesType)
	{
		var spotifyReleasesAction = new SpotifyReleasesActionGet(releasesType, forceUpdate);
		_dispatcher.Dispatch(spotifyReleasesAction);
		//await spotifyReleasesAction.CompletionSource.Task;
	}
}