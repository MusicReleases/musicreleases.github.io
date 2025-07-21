using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Date;

public partial class MenuDateYear
{
	private bool Loading => SpotifyArtistState.Value.LoadingAny() || SpotifyReleaseState.Value.LoadingAny();
	private Dictionary<int, SortedSet<int>>? FilteredYearMonth => SpotifyFilterService.FilteredYearMonth;

	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += OnFilterOrDataChanged;
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
	private void OnFilterOrDataChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= OnFilterOrDataChanged;
	}
}
