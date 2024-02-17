using JakubKastner.MusicReleases.Base;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Date;

public partial class MenuDate
{
	//private SortedSet<SpotifyApi.Objects.Artist>? _artists;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		if (!_baseLoginController.IsUserLoggedIn())
		{
			return;
		}

		var serviceType = _baseLoginController.GetServiceType();
		if (serviceType == Enums.ServiceType.Spotify)
		{
			// TODO show loader
			// display playlists
			//_artists = await _spotifyControllerArtist.GetUserFollowedArtists();
		}
	}

	/*private async Task UpdateArtists()
	{
		_artists = await _spotifyControllerArtist.GetUserFollowedArtists();
		StateHasChanged();
	}*/
}
