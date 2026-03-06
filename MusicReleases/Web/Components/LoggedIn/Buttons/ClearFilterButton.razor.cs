using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Buttons;

public partial class ClearFilterButton : IDisposable
{
	[Inject]
	private ISpotifyFilterUrlServiceOld SpotifyFilterUrlService { get; set; } = default!;

	[Inject]
	private ISpotifyReleaseFilterService SpotifyReleaseFilterService { get; set; } = default!;

	[Inject]
	private NavigationManager NavManager { get; set; } = default!;


	[Parameter, EditorRequired]
	public required ClearFilterButtonComponent ButtonType { get; set; }

	[Parameter]
	public string? Class { get; set; }


	private string ButtonClass => $"filter-clear {ButtonType.ToLowerString()} {Class}";

	private string ButtonText => ButtonType == ClearFilterButtonComponent.All ? (IsFilterActive ? "Clear all filters" : "No filters applied") : string.Empty;

	private string ButtonTitle => ButtonType switch
	{
		ClearFilterButtonComponent.All => IsFilterActive ? "Clear all filters" : "No filters applied",
		ClearFilterButtonComponent.Artists => IsFilterActive ? "Clear artist filter" : "No artist filter applied",
		ClearFilterButtonComponent.Date => IsFilterActive ? "Clear date filter" : "No date filter applied",
		ClearFilterButtonComponent.Releases => IsFilterActive ? "Default releases filter" : "No release filters applied",
		_ => throw new NotImplementedException(),
	};

	private FilterType FilterType => ButtonType switch
	{
		ClearFilterButtonComponent.All => FilterType.Any,
		ClearFilterButtonComponent.Artists => FilterType.Artist,
		ClearFilterButtonComponent.Date => FilterType.Date,
		ClearFilterButtonComponent.Releases => FilterType.Advanced,
		_ => throw new NotImplementedException(),
	};

	private bool IsFilterActive => SpotifyReleaseFilterService.IsFilterActive(FilterType);

	private LucideIcon Icon => IsFilterActive ? LucideIcon.FunnelX : LucideIcon.Funnel;


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

	private async Task ClearFilter()
	{
		if (!IsFilterActive)
		{
			return;
		}
		SpotifyReleaseFilterService.ClearFilter(FilterType);

		/*var url = await SpotifyFilterUrlService.ClearFilter(ButtonType);
		NavManager.NavigateTo(url);*/
	}
}