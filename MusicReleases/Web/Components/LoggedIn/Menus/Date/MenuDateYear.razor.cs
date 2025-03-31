using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Date;

public partial class MenuDateYear
{
	private bool Loading => SpotifyArtistState.Value.LoadingAny() || SpotifyReleaseState.Value.LoadingAny();
	private Dictionary<int, SortedSet<int>>? FilteredYearMonth => SpotifyFilterState.Value.FilteredYearMonth;

	protected override void OnInitialized()
	{
		base.OnInitialized();

		if (!ApiLoginService.IsUserLoggedIn())
		{
			return;
		}

		var serviceType = ApiLoginService.GetServiceType();
		if (serviceType == ServiceType.Spotify)
		{
			// TODO show loader
			// display playlists
			//_artists = await _spotifyControllerArtist.GetUserFollowedArtists();
		}
	}
}
