using JakubKastner.SpotifyApi.Objects;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Artists;

public partial class MenuArtists
{
	private ISet<SpotifyArtist>? Artists => SpotifyFilterService.FilteredArtists;
	private bool Error => SpotifyArtistState.Value.Error;
	private bool Loading => SpotifyArtistState.Value.LoadingAny();

	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += OnFilterOrDataChanged;
		base.OnInitialized();

		var userLoggedIn = ApiLoginService.IsUserLoggedIn();

		if (!userLoggedIn)
		{
			return;
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