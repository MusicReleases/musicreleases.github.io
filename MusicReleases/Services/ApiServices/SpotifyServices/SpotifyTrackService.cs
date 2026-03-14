using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services;
using JakubKastner.SpotifyApi.Services.Api;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyTrackService(ISpotifyApiTrackService spotifyTrackService, IApiUserClient spotifyUserClient) : ISpotifyTrackService
{
	private readonly ISpotifyApiTrackService _spotifyTrackService = spotifyTrackService;
	private readonly IApiUserClient _spotifyUserClient = spotifyUserClient;

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
