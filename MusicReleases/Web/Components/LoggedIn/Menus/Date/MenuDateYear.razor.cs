using static JakubKastner.MusicReleases.Base.Enums;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Date;

public partial class MenuDateYear
{
	private bool Loading => LoaderService.IsLoading(LoadingType.Artists) || LoaderService.IsLoading(LoadingType.Releases);
	private Dictionary<int, SortedSet<int>>? FilteredYearMonth => SpotifyFilterService.FilteredYearMonth;

	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += LoadingStateChanged;
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

	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= LoadingStateChanged;
		SpotifyFilterService.OnFilterOrDataChanged -= OnFilterOrDataChanged;
	}

	private void LoadingStateChanged()
	{
		InvokeAsync(StateHasChanged);
	}
	private void OnFilterOrDataChanged()
	{
		InvokeAsync(StateHasChanged);
	}
}
