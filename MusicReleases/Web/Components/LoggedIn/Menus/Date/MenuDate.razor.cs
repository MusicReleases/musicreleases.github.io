using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Date;

public partial class MenuDate
{
	//private SortedSet<SpotifyApi.Objects.Artist>? _artists;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		if (!_apiLoginController.IsUserLoggedIn())
		{
			return;
		}

		var serviceType = _apiLoginController.GetServiceType();
		if (serviceType == ServiceType.Spotify)
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
