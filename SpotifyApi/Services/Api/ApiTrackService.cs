using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Services.Api;

internal class ApiTrackService(ISpotifyApiClient client) : IApiTrackService
{
	private readonly ISpotifyApiClient _client = client;

	public async Task<ISet<SpotifyTrack>> GetReleaseTracksFromApi(SpotifyRelease release)
	{
		var tracks = new SortedSet<SpotifyTrack>();
		var tracksFromApi = await GetReleaseTracksApi(release.Id);

		if (tracksFromApi == null)
		{
			return tracks;
		}

		foreach (var trackApi in tracksFromApi)
		{
			var track = new SpotifyTrack(trackApi, release);

			tracks.Add(track);
		}

		return tracks;
	}

	private async Task<IList<SimpleTrack>?> GetReleaseTracksApi(string releaseId)
	{

		var request = new AlbumTracksRequest
		{
			Limit = ApiRequestLimit.ReleaseTracks,
			// TODO market
		};

		var spotifyClient = _client.GetClient();

		try
		{
			var response = await spotifyClient.Albums.GetTracks(releaseId, request);
			var tracks = await spotifyClient.PaginateAll(response);
			return tracks;
		}
		catch (APIException e)
		{
			// TODO try again
			// Prints: invalid id
			Console.WriteLine(e.Message);
			// Prints: BadRequest
			Console.WriteLine(e.Response?.StatusCode); // 503 = service unavailable
			await Task.Delay(1000);
		}

		// TODO null
		return null;
	}
}
