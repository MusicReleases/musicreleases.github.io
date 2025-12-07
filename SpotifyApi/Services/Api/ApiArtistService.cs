using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Services.Api;

internal class ApiArtistService(ISpotifyApiClient client) : IApiArtistService
{
	private readonly ISpotifyApiClient _client = client;

	public async Task<List<SpotifyArtist>> GetFollowed(CancellationToken ct = default)
	{
		var request = new FollowOfCurrentUserRequest(FollowOfCurrentUserRequest.Type.Artist)
		{
			Limit = ApiRequestLimit.UserFollowedArtists,
		};

		var spotifyClient = _client.GetClient();
		var response = await spotifyClient.Follow.OfCurrentUser(request);
		var artistsAsync = spotifyClient.Paginate(response.Artists, s => s.Artists);

		var artists = new List<SpotifyArtist>();
		await foreach (var artistApi in artistsAsync.WithCancellation(ct))
		{
			artists.Add(new SpotifyArtist(artistApi));
		}

		return artists;
	}
}