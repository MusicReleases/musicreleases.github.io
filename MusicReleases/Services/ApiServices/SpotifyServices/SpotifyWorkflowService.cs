using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.State.Spotify;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.Services;
using JakubKastner.SpotifyApi.SpotifyEnums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyWorkflowService(ISpotifyReleaseState spotifyReleaseState, ISpotifyPlaylistState spotifyPlaylistState, ILoaderService loaderService, ISpotifyArtistService spotifyArtistService, ISpotifyReleaseService spotifyReleaseService, ISpotifyPlaylistService spotifyPlaylistService) : ISpotifyWorkflowService
{
	private readonly ISpotifyReleaseState _spotifyReleaseState = spotifyReleaseState;
	private readonly ISpotifyPlaylistState _spotifyPlaylistState = spotifyPlaylistState;

	private readonly ILoaderService _loaderService = loaderService;

	private readonly ISpotifyArtistService _spotifyArtistService = spotifyArtistService;
	private readonly ISpotifyReleaseService _spotifyReleaseService = spotifyReleaseService;
	private readonly ISpotifyPlaylistService _spotifyPlaylistService = spotifyPlaylistService;

	private const int DateForceHours = 24;

	public async Task StartLoadingAll(MainReleasesType releaseType, bool forceUpdate)
	{
		await StartLoadingArtistsWithReleases(releaseType, forceUpdate);
		await StartLoadingPlaylistsWithTracks(forceUpdate);
	}


	// playlists
	public async Task StartLoadingPlaylistsWithTracks(bool forceUpdate)
	{
		await StartLoadingPlaylists(forceUpdate);

		var playlists = _spotifyPlaylistState.Playlists;
		if (playlists is null)
		{
			return;
		}
		await StartLoadingPlaylistsTracks(forceUpdate, playlists.ToHashSet());
	}

	private async Task StartLoadingPlaylists(bool forceUpdate)
	{
		Console.WriteLine("workflow: playlists - start");

		await _spotifyPlaylistService.LoadAndSync(forceUpdate);

		Console.WriteLine("workflow: playlists - end");
	}

	private async Task StartLoadingPlaylistsTracks(bool forceUpdate, ISet<SpotifyPlaylist> playlists)
	{
		Console.WriteLine("workflow: playlist tracks - start");
		// TODO load playlists tracks
		/*var forceUpdateAuto = false;

		if (_loaderService.IsLoading(LoadingType.Playlists) || _loaderService.IsLoading(LoadingType.PlaylistTracks))
		{
			return;
		}
		if (!forceUpdate)
		{
			forceUpdateAuto = ForceUpdate(/*todo*//*);
		}

		if (forceUpdate || forceUpdateAuto)
		{
			// TODO load playlists tracks
		}*/
		Console.WriteLine("workflow: playlist tracks - end");
	}


	// artists
	public async Task StartLoadingArtistsWithReleases(MainReleasesType releaseType, bool forceUpdate)
	{
		await StartLoadingArtists(forceUpdate);
		await StartLoadingReleases(releaseType, forceUpdate);
	}

	private async Task StartLoadingArtists(bool forceUpdate)
	{
		Console.WriteLine("workflow: artists - start");

		await _spotifyArtistService.Get(forceUpdate);

		Console.WriteLine("workflow: artists - end");
	}

	public async Task StartLoadingReleases(MainReleasesType releaseType, bool forceUpdate)
	{
		Console.WriteLine("workflow: releases - start");

		await _spotifyReleaseService.Get(releaseType, forceUpdate);

		Console.WriteLine("workflow: releases - end");
	}

	private bool ForceUpdate<T, U>(SpotifyUserList<T, U>? userList, MainReleasesType? releaseType = null) where T : SpotifyIdNameUrlObject where U : SpotifyUserListUpdate
	{
		if (userList is null || userList.List is null || userList.Update is null || userList.List.Count < 1)
		{
			return true;
		}

		DateTime? lastUpdate;

		if (releaseType.HasValue && userList.Update is SpotifyUserListUpdateRelease userListUpdateRelease)
		{
			lastUpdate = ISpotifyApiReleaseService.GetLastTimeUpdate(userListUpdateRelease, releaseType.Value);
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

	public async Task Update(UpdateButtonComponent updateType, MainReleasesType releaseType)
	{
		switch (updateType)
		{
			case UpdateButtonComponent.Artists:
				// TODO load only releases for new artists
				await StartLoadingArtistsWithReleases(releaseType, true);
				// TODO !!!!!!! set old update date for other release types - for update later
				break;
			case UpdateButtonComponent.Releases:
				// TODO load only releases without updating artists
				await StartLoadingArtistsWithReleases(releaseType, true);
				break;
			case UpdateButtonComponent.Playlists:
				await StartLoadingPlaylistsWithTracks(true);
				break;
			default:
				throw new NotSupportedException(nameof(Type));
		}
	}
}