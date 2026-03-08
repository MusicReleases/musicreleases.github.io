using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Content;

public partial class ReleaseFilterButton : IDisposable
{
	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;


	[Parameter, EditorRequired]
	public required ChipFilterButtonComponent ButtonType { get; set; }

	[Parameter, EditorRequired]
	public required ReleaseAdvancedFilter FilterType { get; set; }


	private string ButtonClass => $"filter-releases {ButtonType.ToLowerString()}";

	private string ButtonTitle => $"{(IsFilterActive ? "Hide" : "Show")} {FilterType.ToFriendlyString(false)}";

	private string ButtonText => FilterType.ToFriendlyString(true);

	private bool IsFilterActive => SpotifyReleaseFilterService.IsAdvancedFilterActive(FilterType);

	protected override void OnInitialized()
	{
		if (FilterType == ReleaseAdvancedFilter.All)
		{
			throw new NotImplementedException();
		}

		SpotifyReleaseFilterService.OnFilterChanged += StateChanged;
	}

	public void Dispose()
	{
		SpotifyReleaseFilterService.OnFilterChanged -= StateChanged;
		GC.SuppressFinalize(this);
	}

	private void StateChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private async Task ChangeFilter()
	{
		SpotifyReleaseFilterService.ToggleAdvancedFilter(FilterType);
	}
}