using JakubKastner.MusicReleases.Spotify.Tasks;
using JakubKastner.SpotifyApi.Releases;
using JakubKastner.SpotifyApi.Tracks;
using JakubKastner.SpotifyApi.User;

namespace JakubKastner.MusicReleases.Spotify.Tracks;

internal class SpotifyTrackService(IBackgroundTaskManagerService2 taskManager, ISpotifyTrackClient apiTrackClient, ISpotifyUserClient spotifyUserClient) : ISpotifyTrackService
{
	private readonly IBackgroundTaskManagerService2 _taskManager = taskManager;
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

		await _taskManager.Run(BackgroundTaskType.ReleaseTracksGet, "Getting release tracks", $"Getting tracks from {release.ReleaseType.ToFriendlyString()} '{release.Name}'", async task =>
		{
			await task.RunStep("Loading from API", BackgroundTaskCategory.GetApi, async ct =>
			{
				// get api
				release.Tracks = [.. await _apiTrackClient.GetReleaseTracks(release, ct)];

				task.AddLink(release.ReleaseType.ToFriendlyString(), $"{release.ReleaseType.ToFriendlyString()} '{release.Name}'", release);
			});

			// save db
			//await _dbSpotifyArtistReleaseService.Save(userId, release.Tracks);
		});



		// display
		OnTracksDataChanged?.Invoke();
	}
}
