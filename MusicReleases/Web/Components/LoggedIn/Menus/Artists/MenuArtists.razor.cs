using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Artists;

public partial class MenuArtists
{
	private ISet<SpotifyArtist>? Artists => SpotifyFilterService.FilteredArtists;
	private bool Loading => LoaderService.IsLoading(MusicReleases.Base.Enums.LoadingType.Artists);

	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += LoadingStateChanged;
		SpotifyFilterService.OnFilterOrDataChanged += OnFilterOrDataChanged;
		base.OnInitialized();

		var userLoggedIn = ApiLoginService.IsUserLoggedIn();

		if (!userLoggedIn)
		{
			return;
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