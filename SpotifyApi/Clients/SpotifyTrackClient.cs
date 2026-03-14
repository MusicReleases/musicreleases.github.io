using JakubKastner.SpotifyApi.Store;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Clients;

internal class SpotifyTrackClient(ISpotifyClientStore client) : ISpotifyTrackClient
{
	private readonly ISpotifyClientStore _client = client;

	public async Task<ISet<SpotifyTrack>> GetReleaseTracks(SpotifyRelease release, CancellationToken ct = default)
	{
		var request = new AlbumTracksRequest
		{
			Limit = ApiRequestLimit.ReleaseTracks,
		};

		var spotifyClient = _client.GetClient();
		var response = await spotifyClient.Albums.GetTracks(release.Id, request, ct);
		var tracksAsync = spotifyClient.Paginate(response, cancel: ct);

		var tracks = new SortedSet<SpotifyTrack>();
		await foreach (var trackApi in tracksAsync.WithCancellation(ct))
		{
			var track = trackApi.ToObject(release);
			tracks.Add(track);
		}

		return tracks;
	}
}