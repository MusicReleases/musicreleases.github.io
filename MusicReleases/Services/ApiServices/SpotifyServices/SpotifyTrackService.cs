using JakubKastner.SpotifyApi.Clients;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Services.ApiServices.SpotifyServices;

public class SpotifyTrackService(ISpotifyTrackClient apiTrackClient, ISpotifyUserClient spotifyUserClient) : ISpotifyTrackService
{
	private readonly ISpotifyTrackClient _apiTrackClient = apiTrackClient;
	private readonly ISpotifyUserClient _spotifyUserClient = spotifyUserClient;

	public event Action? OnTracksDataChanged;

	public async Task Get(SpotifyRelease release)
	{
		if (release.Tracks is not null && release.Tracks.Count > 0)
		{
			// tracks already loaded
			return;
		}

		// get api
		release.Tracks = [.. await _apiTrackClient.GetReleaseTracks(release)];

		// save db
		//await _dbSpotifyArtistReleaseService.Save(userId, release.Tracks);

		// display
		OnTracksDataChanged?.Invoke();
	}
}
