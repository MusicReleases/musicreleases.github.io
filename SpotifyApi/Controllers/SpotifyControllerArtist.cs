using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerArtist(IControllerApiArtist controllerApiArtist, ISpotifyControllerUser controllerUser) : ISpotifyControllerArtist
{
	private readonly IControllerApiArtist _controllerApiArtist = controllerApiArtist;
	private readonly ISpotifyControllerUser _controllerUser = controllerUser;

	// get list of user followed artists
	public async Task<ISet<SpotifyArtist>> GetUserFollowedArtists()
	{
		var user = _controllerUser.GetUserRequired();
		var artistsApi = await _controllerApiArtist.GetUserFollowedArtistsFromApi();

		user.FollowedArtists ??= new(artistsApi, DateTime.Now);
		var artists = user.FollowedArtists.List;

		return artists;
	}
}