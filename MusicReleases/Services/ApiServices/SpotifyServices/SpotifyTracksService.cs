using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.MusicReleases.Services.DatabaseServices.SpotifyServices;
using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Objects.Base;
using JakubKastner.SpotifyApi.Services;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyTracksService(ISpotifyTrackService spotifyTrackService, ISpotifyUserService spotifyUserService, IDbSpotifyArtistReleaseService dbSpotifyArtistReleaseService, ILoaderService loaderService) : ISpotifyTracksService
{
	private readonly ISpotifyTrackService _spotifyTrackService = spotifyTrackService;
	private readonly ISpotifyUserService _spotifyUserService = spotifyUserService;
	private readonly IDbSpotifyArtistReleaseService _dbSpotifyArtistReleaseService = dbSpotifyArtistReleaseService;
	private readonly ILoaderService _loaderService = loaderService;

	public SpotifyUserList<SpotifyRelease, SpotifyUserListUpdateRelease>? Releases { get; private set; } = null;

	public event Action? OnTracksDataChanged;

	public async Task Get(SpotifyRelease release)
	{
		if (release.Tracks is not null && release.Tracks.Count > 0)
		{
			// tracks already loaded
			return;
		}

		// get api
		await _spotifyTrackService.GetReleaseTracks(release);

		// save db
		//await _dbSpotifyArtistReleaseService.Save(userId, release.Tracks);

		// display
		OnTracksDataChanged?.Invoke();
	}
}
