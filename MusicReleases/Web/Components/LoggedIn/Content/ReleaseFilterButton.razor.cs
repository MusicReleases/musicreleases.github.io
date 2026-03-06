using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseFilterButton : IDisposable
{
	[Inject]
	private ISpotifyFilterUrlServiceOld SpotifyFilterUrlService { get; set; } = default!;

	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;

	[Inject]
	private NavigationManager NavManager { get; set; } = default!;


	[Parameter, EditorRequired]
	public required ChipFilterButtonComponent ButtonType { get; set; }

	[Parameter, EditorRequired]
	public required ReleaseAdvancedFilter FilterType { get; set; }


	private string ButtonClass => $"filter-releases {ButtonType.ToLowerString()}";

	private string ButtonTitle => $"{(IsFilterActive ? "Hide" : "Show")} {GetTypeName(false)}";

	private string ButtonText => GetTypeName(true);

	private bool IsFilterActive => SpotifyReleaseFilterService.IsAdvancedFilterActive(FilterType);

	protected override void OnInitialized()
	{
		SpotifyReleaseFilterService.OnFilterOrDataChanged += StateChanged;
	}

	public void Dispose()
	{
		SpotifyReleaseFilterService.Dispose();
		SpotifyReleaseFilterService.OnFilterOrDataChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private string GetTypeName(bool capitalizeFirstLetter)
	{
		if (FilterType == ReleaseAdvancedFilter.All)
		{
			throw new NotImplementedException();
		}

		var name = FilterType.ToFriendlyString(capitalizeFirstLetter);
		/*if (FilterType == ReleasesFilters.EPs && SpotifyReleaseFilterService.Filter?.ReleaseType == MainReleasesType.Appears)
		{
			name = $"Albums and {name}";
		}*/

		return name;
	}

	private async Task ChangeFilter()
	{
		SpotifyReleaseFilterService.ToggleAdvancedFilter(FilterType);

		/*var url = await SpotifyFilterUrlService.GetFilterUrl(FilterType, !IsFilterActive());
		NavManager.NavigateTo(url);*/
	}

	/*private bool IsFilterActive()

	{
		// this names must be same as in the URL and in Enums.ReleasesFilters
		var filterProperty = SpotifyReleaseFilterService.Filter!.Advanced.GetType().GetProperty(FilterType.ToString());
		if (filterProperty is null || filterProperty.PropertyType != typeof(bool))
		{
			throw new NotSupportedException(nameof(filterProperty));
		}

		return (bool)filterProperty.GetValue(SpotifyReleaseFilterService.Filter.Advanced)!;
	}*/
}