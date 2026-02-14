using JakubKastner.MusicReleases.Enums;
using JakubKastner.MusicReleases.Services.BaseServices;
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
	public required ReleasesFilters LeftType { get; set; }

	[Parameter, EditorRequired]
	public required string LeftTitle { get; set; }

	[Parameter, EditorRequired]
	public required string LeftText { get; set; }

	[Parameter, EditorRequired]
	public required ReleasesFilters RightType { get; set; }

	[Parameter, EditorRequired]
	public required string RightTitle { get; set; }

	[Parameter, EditorRequired]
	public required string RightText { get; set; }


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

	private string ButtonClass(ReleasesFilters type, string? customClass = null)
	{
		var active = IsFilterActive(type);
		var buttonClass = $"rounded-m chips {customClass}{(active ? " active" : string.Empty)}";
		return buttonClass;
	}

	private string ButtonTitle(ReleasesFilters type, string title)
	{
		var active = IsFilterActive(type);
		var buttonTitle = $"{(active ? "Hide" : "Show")} {title}";
		return buttonTitle;
	}

	private async Task ChangeFilter(ReleasesFilters type)
	{
		var url = await SpotifyFilterUrlService.GetFilterUrl(type, !IsFilterActive(type));
		NavManager.NavigateTo(url);
	}

	private bool IsFilterActive(ReleasesFilters type)
	{
		// this names must be same as in the URL and in Enums.ReleasesFilters
		var filterProperty = SpotifyFilterService.Filter!.Advanced.GetType().GetProperty(type.ToString());
		if (filterProperty is null || filterProperty.PropertyType != typeof(bool))
		{
			throw new NotSupportedException(nameof(filterProperty));
		}

		return (bool)filterProperty.GetValue(SpotifyFilterService.Filter.Advanced)!;
	}
}