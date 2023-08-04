using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerArtist : ISpotifyControllerArtist
{
	private readonly IControllerApiArtist _controllerApiArtist;
	private readonly SpotifyUser _user;

	public SpotifyControllerArtist(IControllerApiArtist controllerApiArtist, SpotifyUser user)
	{
		_controllerApiArtist = controllerApiArtist;
		_user = user;
	}

	// get list of user followed artists
	public async Task<ISet<SpotifyArtist>> GetUserFollowedArtists()
	{
		return _user.FollowedArtists ??= await _controllerApiArtist.GetUserFollowedArtistsFromApi();
	}
}