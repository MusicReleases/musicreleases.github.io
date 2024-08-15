using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerArtist(IControllerApiArtist controllerApiArtist, ISpotifyControllerUser controllerUser) : ISpotifyControllerArtist
{
	private readonly IControllerApiArtist _controllerApiArtist = controllerApiArtist;
	private readonly ISpotifyControllerUser _controllerUser = controllerUser;

	public async Task<SpotifyUserList<SpotifyArtist>> GetUserFollowedArtists(SpotifyUserList<SpotifyArtist>? existingArtists = null, bool forceUpdate = false)
	{
		if (existingArtists is null)
		{
			forceUpdate = true;
		}
		else
		{
			var dateTimeDifference = DateTime.Now - existingArtists.LastUpdate;

			if (dateTimeDifference.TotalHours >= 24)
			{
				// force update every 24 hours
				forceUpdate = true;
			}
		}

		if (!forceUpdate)
		{
			// doesnt need update
			return existingArtists!;
		}

		var artists = await GetUserArtistsApi(forceUpdate);
		return artists;
	}

	private async Task<SpotifyUserList<SpotifyArtist>> GetUserArtistsApi(bool forceUpdate = false)
	{
		var user = _controllerUser.GetUserRequired();

		if (user.FollowedArtists?.List is null || forceUpdate)
		{
			var artistsApi = await _controllerApiArtist.GetUserFollowedArtistsFromApi();
			user.FollowedArtists = new(artistsApi, DateTime.Now);
		}

		var artists = user.FollowedArtists;
		return artists;
	}
}