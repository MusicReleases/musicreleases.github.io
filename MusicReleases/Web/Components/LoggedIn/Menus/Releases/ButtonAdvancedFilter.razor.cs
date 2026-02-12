using JakubKastner.MusicReleases.Enums;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Releases;

public partial class ButtonAdvancedFilter
{
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


	private string ButtonClass(ReleasesFilters type, string customClass)
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

	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += OnFilterOrDataChanged;
	}

	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= OnFilterOrDataChanged;
		GC.SuppressFinalize(this);
	}

	private void OnFilterOrDataChanged()
	{
		InvokeAsync(StateHasChanged);
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