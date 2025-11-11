using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.Services;
using static JakubKastner.MusicReleases.Base.Enums;
using static JakubKastner.SpotifyApi.Base.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyWorkflowService(ISpotifyFilterService spotifyFilterService, ILoaderService loaderService, ISpotifyArtistsService spotifyArtistsService, ISpotifyReleasesService spotifyReleasesService, ISpotifyPlaylistsService spotifyPlaylistsService) : ISpotifyWorkflowService
{
	private readonly ISpotifyFilterService _spotifyFilterService = spotifyFilterService;
	private readonly ILoaderService _loaderService = loaderService;

	private readonly ISpotifyArtistsService _spotifyArtistsService = spotifyArtistsService;
	private readonly ISpotifyReleasesService _spotifyReleasesService = spotifyReleasesService;
	private readonly ISpotifyPlaylistsService _spotifyPlaylistsService = spotifyPlaylistsService;

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
		Console.WriteLine("workflow: playlists - start");
		var forceUpdateAuto = false;

		if (_loaderService.IsLoading(LoadingType.Playlists) || _loaderService.IsLoading(LoadingType.PlaylistTracks))
		{
			return null;
		}
		if (!forceUpdate)
		{
			forceUpdateAuto = ForceUpdate(_spotifyPlaylistsService.Playlists);
		}

		if (forceUpdate || forceUpdateAuto)
		{
			await _spotifyPlaylistsService.Get(forceUpdate);
		}
		var playlists = _spotifyPlaylistsService.Playlists;
		Console.WriteLine("workflow: playlists - end");
		return playlists;
	}

	private async Task StartLoadingPlaylistsTracks(bool forceUpdate, SpotifyUserList<SpotifyPlaylist, SpotifyUserListUpdatePlaylists> playlists)
	{
		Console.WriteLine("workflow: playlist tracks - start");
		var forceUpdateAuto = false;

		if (_loaderService.IsLoading(LoadingType.Playlists) || _loaderService.IsLoading(LoadingType.PlaylistTracks))
		{
			return;
		}
		if (!forceUpdate)
		{
			forceUpdateAuto = ForceUpdate(_spotifyPlaylistsService.Playlists);
		}

		if (forceUpdate || forceUpdateAuto)
		{
			// TODO load playlists tracks
		}
		Console.WriteLine("workflow: playlist tracks - end");
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

	private async Task<ISet<SpotifyArtist>?> StartLoadingArtists(bool forceUpdate)
	{
		Console.WriteLine("workflow: artists - start");
		var forceUpdateAuto = false;

		if (_loaderService.IsLoading(LoadingType.Artists) || _loaderService.IsLoading(LoadingType.Releases))
		{
			return null;
		}

		if (!forceUpdate)
		{
			forceUpdateAuto = ForceUpdate(_spotifyArtistsService.Artists);
		}
		if (forceUpdate || forceUpdateAuto)
		{
			await _spotifyArtistsService.Get(forceUpdate);
		}
		var artists = _spotifyArtistsService.Artists?.List;
		if (artists is null)
		{
			return null;
		}

		_spotifyFilterService.SetArtists(artists);
		Console.WriteLine("workflow: artists - end");
		return artists;
	}

	public async Task StartLoadingReleases(bool forceUpdate, ReleaseType releaseType, ISet<SpotifyArtist> artists)
	{
		Console.WriteLine("workflow: releases - start");
		var forceUpdateAuto = false;
		if (_loaderService.IsLoading(LoadingType.Artists) || _loaderService.IsLoading(LoadingType.Releases))
		{
			return;
		}

		if (!forceUpdate)
		{
			forceUpdateAuto = ForceUpdate(_spotifyReleasesService.Releases, releaseType);
		}

		if (forceUpdate || forceUpdateAuto)
		{
			await _spotifyReleasesService.Get(releaseType, artists, forceUpdate);
		}
		var releases = _spotifyReleasesService.Releases?.List;
		if (releases is null)
		{
			return;
		}

		// filter releases
		_spotifyFilterService.SetReleases(releases);
		Console.WriteLine("workflow: releases - end");
	}

	public bool ForceUpdate<T, U>(SpotifyUserList<T, U>? userList, ReleaseType? releaseType = null) where T : SpotifyIdNameUrlObject where U : SpotifyUserListUpdate
	{
		if (userList is null || userList.List is null || userList.Update is null || userList.List.Count < 1)
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