using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Clients;

internal class SpotifyArtistClient(ISpotifyApiClient client) : ISpotifyArtistClient
{
	private readonly ISpotifyApiClient _client = client;

	public async Task<List<SpotifyArtist>> GetFollowed(CancellationToken ct = default)
	{
		var request = new FollowOfCurrentUserRequest(FollowOfCurrentUserRequest.Type.Artist)
		{
			Limit = ApiRequestLimit.UserFollowedArtists,
		};

		var spotifyClient = _client.GetClient();
		var response = await spotifyClient.Follow.OfCurrentUser(request, ct);
		var artistsAsync = spotifyClient.Paginate(response.Artists, s => s.Artists, cancel: ct);

		var artists = new List<SpotifyArtist>();
		await foreach (var artistApi in artistsAsync.WithCancellation(ct))
		{
			var artist = artistApi.ToObject();
			artists.Add(artist);
		}

		return artists;
	}
}