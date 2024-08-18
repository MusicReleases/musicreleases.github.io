using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerArtist(IControllerApiArtist controllerApiArtist, ISpotifyControllerUser controllerUser) : ISpotifyControllerArtist
{
	private readonly IControllerApiArtist _controllerApiArtist = controllerApiArtist;
	private readonly ISpotifyControllerUser _controllerUser = controllerUser;

	public async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>> GetUserFollowedArtists(SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>? existingArtists = null, bool forceUpdate = false)
	{
		if (existingArtists is null)
		{
			forceUpdate = true;
		}
		else
		{
			if (existingArtists.Update is null)
			{
				throw new NullReferenceException(nameof(existingArtists.Update));
			}

			var dateTimeDifference = DateTime.Now - existingArtists.Update.LastUpdateMain;

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

		var artists = await GetUserArtistsApi(existingArtists, forceUpdate);
		return artists;
	}

	private async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>> GetUserArtistsApi(SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>? existingArtists, bool forceUpdate)
	{
		SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>? artists;
		var user = _controllerUser.GetUserRequired();

		if (existingArtists?.List is null || forceUpdate)
		{
			var artistsApi = await _controllerApiArtist.GetUserFollowedArtistsFromApi();
			artists = new SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateArtists>(artistsApi, new SpotifyUserListUpdateArtists(DateTime.Now));
		}
		else
		{
			artists = existingArtists;
		}

		user.FollowedArtists = artists;

		return artists;
	}
}