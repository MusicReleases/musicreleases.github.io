using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using JakubKastner.SpotifyApi.SpotifyEnums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseFilterButton : IDisposable
{
	[Inject]
	private ISpotifyFilterUrlService SpotifyFilterUrlService { get; set; } = default!;

	[Inject]
	private ISpotifyFilterService SpotifyFilterService { get; set; } = default!;

	[Inject]
	private NavigationManager NavManager { get; set; } = default!;


	[Parameter, EditorRequired]
	public required ChipFilterButtonComponent ButtonType { get; set; }

	[Parameter, EditorRequired]
	public required ReleasesFilters FilterType { get; set; }


	private string ButtonClass => $"filter-releases {ButtonType.ToLowerString()}";

	private string ButtonTitle => $"{(IsFilterActive() ? "Hide" : "Show")} {GetTypeName(false)}";

	private string ButtonText => GetTypeName(true);


	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += StateChanged;
	}

	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private string GetTypeName(bool capitalizeFirstLetter)
	{
		if (FilterType == ReleasesFilters.Clear)
		{
			throw new NotImplementedException();
		}

		var name = FilterType.ToFriendlyString(capitalizeFirstLetter);
		if (FilterType == ReleasesFilters.EPs && SpotifyFilterService.Filter?.ReleaseType == MainReleasesType.Appears)
		{
			name = $"Albums and {name}";
		}

		return name;
	}

	private async Task ChangeFilter()
	{
		var url = await SpotifyFilterUrlService.GetFilterUrl(FilterType, !IsFilterActive());
		NavManager.NavigateTo(url);
	}

	private bool IsFilterActive()
	{
		// this names must be same as in the URL and in Enums.ReleasesFilters
		var filterProperty = SpotifyFilterService.Filter!.Advanced.GetType().GetProperty(FilterType.ToString());
		if (filterProperty is null || filterProperty.PropertyType != typeof(bool))
		{
			throw new NotSupportedException(nameof(filterProperty));
		}

		return (bool)filterProperty.GetValue(SpotifyFilterService.Filter.Advanced)!;
	}
}