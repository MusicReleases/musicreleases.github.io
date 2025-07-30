using JakubKastner.SpotifyApi.Objects;
using JakubKastner.SpotifyApi.Services.Api;

namespace JakubKastner.SpotifyApi.Services;

internal class SpotifyTrackService(IApiTrackService apiTrackService) : ISpotifyTrackService
{
	private readonly IApiTrackService _apiTrackService = apiTrackService;
	public async Task<ISet<SpotifyTrack>> GetReleaseTracks(SpotifyRelease release)
	{
		if (release.Tracks is not null && release.Tracks.Count > 0)
		{
			// tracks already loaded
			return release.Tracks;
		}

		var tracksApi = await _apiTrackService.GetReleaseTracksFromApi(release);
		release.Tracks = [.. tracksApi];
		return release.Tracks;
	}
}