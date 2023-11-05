using JakubKastner.MusicReleases.Base;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Date;

public partial class MenuDate
{
	//private SortedSet<SpotifyApi.Objects.Artist>? _artists;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		var serviceType = _serviceTypeController.GetRequired();
		if (serviceType == Enums.ServiceType.Spotify)
		{
			// TODO show loader
			// display playlists
			if (!_spotifyControllerUser.IsLoggedIn()) return;

			//_artists = await _spotifyControllerArtist.GetUserFollowedArtists();
		}
	}

	/*private async Task UpdateArtists()
	{
		_artists = await _spotifyControllerArtist.GetUserFollowedArtists();
		StateHasChanged();
	}*/
}
