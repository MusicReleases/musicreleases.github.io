using JakubKastner.MusicReleases.Enums;
using JakubKastner.SpotifyApi.Objects;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Artists;

public partial class MenuArtists
{
	[Parameter]
	public string? Class { get; set; }

	private ISet<SpotifyArtist>? Artists => SpotifyFilterService.FilteredArtists;
	private bool Loading => LoaderService.IsLoading(LoadingType.Artists);
	private readonly MenuType _menuType = MenuType.Artists;

	protected override void OnInitialized()
	{
		LoaderService.LoadingStateChanged += LoadingStateChanged;
		SpotifyFilterService.OnFilterOrDataChanged += OnFilterOrDataChanged;
	}
	public void Dispose()
	{
		LoaderService.LoadingStateChanged -= LoadingStateChanged;
		SpotifyFilterService.OnFilterOrDataChanged -= OnFilterOrDataChanged;
		GC.SuppressFinalize(this);
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