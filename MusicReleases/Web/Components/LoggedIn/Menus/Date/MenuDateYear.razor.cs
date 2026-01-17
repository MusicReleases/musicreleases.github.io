using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Date;

public partial class MenuDateYear
{
	[Parameter]
	public string? Class { get; set; }

	private bool Loading => LoaderService.IsLoading(LoadingType.Artists) || LoaderService.IsLoading(LoadingType.Releases);
	private Dictionary<int, SortedSet<int>>? FilteredYearMonth => SpotifyFilterService.FilteredYearMonth;

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
