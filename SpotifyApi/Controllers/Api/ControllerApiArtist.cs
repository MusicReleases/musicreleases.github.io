using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public class ControllerApiArtist
{
	private readonly Client _client;

	public ControllerApiArtist(Client client)
	{
		_client = client;
	}

	public async Task<SortedSet<Artist>> GetUserFollowedArtistsFromApi()
	{
		var artists = new SortedSet<Artist>();
		var artistsFromApi = await GetUserFollowedArtistsApi();

		if (artistsFromApi == null)
		{
			return artists;
		}

		foreach (var artistApi in artistsFromApi)
		{
			var artist = new Artist(artistApi);
			artists.Add(artist);
		}

		return artists;
	}

	private async Task<IList<FullArtist>?> GetUserFollowedArtistsApi()
	{
		var request = new FollowOfCurrentUserRequest(FollowOfCurrentUserRequest.Type.Artist)
		{
			Limit = ApiRequestLimit.UserFollowedArtists,
		};
		var spotifyClient = _client.GetClient();
		var response = await spotifyClient.Follow.OfCurrentUser(request);
		var artistsAsync = spotifyClient.Paginate(response.Artists, (s) => s.Artists);

		var artists = new List<FullArtist>();

		await foreach (var artist in artistsAsync)
		{
			artists.Add(artist);
		}
		return artists;
	}
}