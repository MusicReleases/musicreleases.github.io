﻿using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleaseStore;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyWorkflowService(IDispatcher dispatcher, IState<SpotifyPlaylistState> spotifyPlaylistState, IState<SpotifyPlaylistTrackState> spotifyPlaylistTrackState, IState<SpotifyArtistState> spotifyArtistState, IState<SpotifyReleaseState> spotifyReleasesState) : ISpotifyWorkflowService
{
	private readonly IDispatcher _dispatcher = dispatcher;
	private readonly IState<SpotifyPlaylistState> _spotifyPlaylistState = spotifyPlaylistState;
	private readonly IState<SpotifyPlaylistTrackState> _spotifyPlaylistTrackState = spotifyPlaylistTrackState;
	private readonly IState<SpotifyArtistState> _spotifyArtistState = spotifyArtistState;
	private readonly IState<SpotifyReleaseState> _spotifyReleasesState = spotifyReleasesState;

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
		if (_spotifyPlaylistState.Value.LoadingAny() || _spotifyPlaylistTrackState.Value.LoadingAny())
		{
			return null;
		}

		var spotifyPlaylistsAction = new SpotifyPlaylistActionGet(forceUpdate);
		_dispatcher.Dispatch(spotifyPlaylistsAction);
		await spotifyPlaylistsAction.CompletionSource.Task;

		var playlists = _spotifyPlaylistState.Value.Error ? null : _spotifyPlaylistState.Value.List;
		return playlists;
	}

	private async Task StartLoadingPlaylistsTracks(bool forceUpdate, SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists> playlists)
	{
		if (_spotifyPlaylistState.Value.LoadingAny() || _spotifyPlaylistTrackState.Value.LoadingAny())
		{
			return;
		}

		var spotifyPlaylistsTracksAction = new SpotifyPlaylistTrackActionGet(forceUpdate, playlists);
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
		await StartLoadingReleases(forceUpdate, releaseType, artists);
	}

	private async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>?> StartLoadingArtists(bool forceUpdate)
	{
		if (_spotifyArtistState.Value.LoadingAny() || _spotifyReleasesState.Value.LoadingAny())
		{
			return null;
		}

		var spotifyArtistsAction = new SpotifyArtistActionGet(forceUpdate);
		_dispatcher.Dispatch(spotifyArtistsAction);

		await spotifyArtistsAction.CompletionSource.Task;

		var artists = _spotifyArtistState.Value.Error ? null : _spotifyArtistState.Value.List;
		return artists;
	}

	public async Task StartLoadingReleases(bool forceUpdate, ReleaseType releaseType, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain> artists)
	{
		if (_spotifyArtistState.Value.LoadingAny() || _spotifyReleasesState.Value.LoadingAny())
		{
			return;
		}

		var spotifyReleasesAction = new SpotifyReleaseActionGet(releaseType, forceUpdate, artists);
		_dispatcher.Dispatch(spotifyReleasesAction);

		await spotifyReleasesAction.CompletionSource.Task;
	}
}