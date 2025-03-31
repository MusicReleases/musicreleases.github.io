using Fluxor;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyArtistStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyPlaylistsTracksStore;
using JakubKastner.MusicReleases.Store.ApiStore.SpotifyStore.SpotifyReleaseStore;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.Services;
using static JakubKastner.MusicReleases.Store.FilterStore.SpotifyFilterAction;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyWorkflowService(IDispatcher dispatcher, IState<SpotifyPlaylistState> spotifyPlaylistState, IState<SpotifyPlaylistTrackState> spotifyPlaylistTrackState, IState<SpotifyArtistState> spotifyArtistState, IState<SpotifyReleaseState> spotifyReleasesState) : ISpotifyWorkflowService
{
	private readonly IDispatcher _dispatcher = dispatcher;
	private readonly IState<SpotifyPlaylistState> _spotifyPlaylistState = spotifyPlaylistState;
	private readonly IState<SpotifyPlaylistTrackState> _spotifyPlaylistTrackState = spotifyPlaylistTrackState;
	private readonly IState<SpotifyArtistState> _spotifyArtistState = spotifyArtistState;
	private readonly IState<SpotifyReleaseState> _spotifyReleasesState = spotifyReleasesState;

	private const int DateForceHours = 24;

	public async Task StartLoadingAll(bool forceUpdate, ReleaseType releaseType)
	{
		await StartLoadingArtistsWithReleases(forceUpdate, releaseType);
		await StartLoadingPlaylistsWithTracks(forceUpdate);
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
		if (!forceUpdate)
		{
			forceUpdate = ForceUpdate(_spotifyPlaylistState.Value.List);
		}

		if (forceUpdate)
		{
			var spotifyPlaylistsAction = new SpotifyPlaylistActionGet(forceUpdate);
			_dispatcher.Dispatch(spotifyPlaylistsAction);
			await spotifyPlaylistsAction.CompletionSource.Task;
		}

		var playlists = _spotifyPlaylistState.Value.Error ? null : _spotifyPlaylistState.Value.List;
		return playlists;
	}

	private async Task StartLoadingPlaylistsTracks(bool forceUpdate, SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists> playlists)
	{
		if (_spotifyPlaylistState.Value.LoadingAny() || _spotifyPlaylistTrackState.Value.LoadingAny())
		{
			return;
		}
		if (!forceUpdate)
		{
			forceUpdate = ForceUpdate(_spotifyPlaylistTrackState.Value.List);
		}

		if (forceUpdate)
		{
			// TODO load playlists tracks
			/*var spotifyPlaylistsTracksAction = new SpotifyPlaylistTrackActionGet(forceUpdate, playlists);
			_dispatcher.Dispatch(spotifyPlaylistsTracksAction);
			await spotifyPlaylistsTracksAction.CompletionSource.Task;*/
		}
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

		if (!forceUpdate)
		{
			forceUpdate = ForceUpdate(_spotifyArtistState.Value.List);
		}
		if (forceUpdate)
		{
			var spotifyArtistsAction = new SpotifyArtistActionGet(forceUpdate);
			_dispatcher.Dispatch(spotifyArtistsAction);
			await spotifyArtistsAction.CompletionSource.Task;
		}

		var artists = _spotifyArtistState.Value.Error ? null : _spotifyArtistState.Value.List;
		return artists;
	}

	public async Task StartLoadingReleases(bool forceUpdate, ReleaseType releaseType, SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain> artists)
	{
		if (_spotifyArtistState.Value.LoadingAny() || _spotifyReleasesState.Value.LoadingAny())
		{
			return;
		}

		if (!forceUpdate)
		{
			forceUpdate = ForceUpdate(_spotifyReleasesState.Value.List, releaseType);
		}

		if (forceUpdate)
		{
			var spotifyReleasesAction = new SpotifyReleaseActionGet(releaseType, forceUpdate, artists);
			_dispatcher.Dispatch(spotifyReleasesAction);
			await spotifyReleasesAction.CompletionSource.Task;
		}

		var releases = _spotifyReleasesState.Value.Error ? null : _spotifyReleasesState.Value.List.List;
		if (releases is null)
		{
			return;
		}

		FilterReleases(releases);
	}

	private void FilterReleases(ISet<SpotifyRelease> releases)
	{
		_dispatcher.Dispatch(new LoadReleasesAction(releases));
	}

	public bool ForceUpdate<T, U>(SpotifyUserList<T, U> userList, ReleaseType? releaseType = null) where T : SpotifyIdNameObject where U : SpotifyUserListUpdate
	{
		if (userList.List is null || userList.Update is null || userList.List.Count < 1)
		{
			return true;
		}

		DateTime? lastUpdate;

		if (releaseType.HasValue && userList.Update is SpotifyUserListUpdateRelease userListUpdateRelease)
		{
			lastUpdate = ISpotifyReleaseService.GetLastTimeUpdate(userListUpdateRelease, releaseType.Value);
		}
		else if (userList.Update is SpotifyUserListUpdateMain userListUpdateMain)
		{
			lastUpdate = userListUpdateMain.LastUpdateMain;
		}
		else
		{
			throw new NotSupportedException(nameof(ForceUpdate));
		}

		var dateTimeDifference = DateTime.Now - lastUpdate;

		if (!dateTimeDifference.HasValue || dateTimeDifference.Value.TotalHours >= DateForceHours)
		{
			return true;
		}
		return false;
	}
}