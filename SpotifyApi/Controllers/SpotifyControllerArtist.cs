using JakubKastner.SpotifyApi.Controllers.Api;
using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.SpotifyApi.Controllers;

public class SpotifyControllerArtist(IControllerApiArtist controllerApiArtist, ISpotifyControllerUser controllerUser) : ISpotifyControllerArtist
{
	private readonly IControllerApiArtist _controllerApiArtist = controllerApiArtist;
	private readonly ISpotifyControllerUser _controllerUser = controllerUser;

	public async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>> GetUserFollowedArtists(SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>? existingArtists = null, bool forceUpdate = false)
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

	private async Task<SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>> GetUserArtistsApi(SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>? existingArtists, bool forceUpdate)
	{
		SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>? artists;
		var user = _controllerUser.GetUserRequired();

		if (existingArtists?.List is null || forceUpdate)
		{
			var artistsApi = await _controllerApiArtist.GetUserFollowedArtistsFromApi();
			artists = new SpotifyUserList<SpotifyArtist, SpotifyUserListUpdateMain>(artistsApi, new SpotifyUserListUpdateMain(DateTime.Now));
		}
		else
		{
			artists = existingArtists;
		}

		user.FollowedArtists = artists;

		return artists;
	}
}