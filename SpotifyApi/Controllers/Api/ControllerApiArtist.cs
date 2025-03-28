using JakubKastner.SpotifyApi.Base;
using JakubKastner.SpotifyApi.Objects;
using SpotifyAPI.Web;

namespace JakubKastner.SpotifyApi.Controllers.Api;

public class ControllerApiArtist(ISpotifyApiClient client) : IControllerApiArtist
{
	private readonly ISpotifyApiClient _client = client;

	public async Task<ISet<SpotifyArtist>> GetUserFollowedArtistsFromApi()
	{
		var artistsFromApi = await GetUserFollowedArtistsApi();
		var artists = new SortedSet<SpotifyArtist>();

		if (artistsFromApi is null)
		{
			return artists;
		}

		foreach (var artistApi in artistsFromApi)
		{
			var artist = new SpotifyArtist(artistApi);
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
		var artistsAsync = spotifyClient.Paginate(response.Artists, s => s.Artists);

		var artists = new List<FullArtist>();

		await foreach (var artist in artistsAsync)
		{
			artists.Add(artist);
		}
		return artists;
	}
}