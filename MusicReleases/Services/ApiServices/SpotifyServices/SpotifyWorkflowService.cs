using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.Enums;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyWorkflowService(ISpotifyArtistService spotifyArtistService, ISpotifyReleaseService spotifyReleaseService, ISpotifyPlaylistService spotifyPlaylistService) : ISpotifyWorkflowService
{
	private readonly ISpotifyArtistService _spotifyArtistService = spotifyArtistService;
	private readonly ISpotifyReleaseService _spotifyReleaseService = spotifyReleaseService;
	private readonly ISpotifyPlaylistService _spotifyPlaylistService = spotifyPlaylistService;

	public async Task StartLoadingAll(ReleaseGroup releaseType, bool forceUpdate)
	{
		await StartLoadingArtistsWithReleases(releaseType, forceUpdate);
		await StartLoadingPlaylistsWithTracks(forceUpdate);
	}


	// playlists
	public async Task StartLoadingPlaylistsWithTracks(bool forceUpdate)
	{
		await StartLoadingPlaylists(forceUpdate);
		await StartLoadingPlaylistsTracks(forceUpdate);
	}

	private async Task StartLoadingPlaylists(bool forceUpdate)
	{
		Console.WriteLine("workflow: playlists - start");

		await _spotifyPlaylistService.LoadAndSync(forceUpdate);

		Console.WriteLine("workflow: playlists - end");
	}

	private async Task StartLoadingPlaylistsTracks(bool forceUpdate)
	{
		//Console.WriteLine("workflow: playlist tracks - start");

		// TODO load playlists tracks

		//Console.WriteLine("workflow: playlist tracks - end");
	}


	// artists
	public async Task StartLoadingArtistsWithReleases(ReleaseGroup releaseType, bool forceUpdate)
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

	public async Task StartLoadingReleases(ReleaseGroup releaseType, bool forceUpdate)
	{
		Console.WriteLine("workflow: releases - start");

		await _spotifyReleaseService.Get(releaseType, forceUpdate);

		Console.WriteLine("workflow: releases - end");
	}

	public async Task Update(UpdateButtonComponent updateType, ReleaseGroup releaseType)
	{
		switch (updateType)
		{
			case UpdateButtonComponent.Artists:
				// TODO load only releases for new artists
				// TODO !!!!!!! set old update date for other release types - for update later
				await StartLoadingArtistsWithReleases(releaseType, true);
				break;
			case UpdateButtonComponent.Releases:
				// load only releases without updating artists
				await StartLoadingReleases(releaseType, true);
				break;
			case UpdateButtonComponent.Playlists:
				await StartLoadingPlaylistsWithTracks(true);
				break;
			default:
				throw new NotSupportedException(nameof(Type));
		}
	}
}