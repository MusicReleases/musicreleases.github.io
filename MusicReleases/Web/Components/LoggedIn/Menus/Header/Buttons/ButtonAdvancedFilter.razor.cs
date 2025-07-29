using JakubKastner.MusicReleases.Base;
using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Header.Buttons;

public partial class ButtonAdvancedFilter
{

	[Parameter, EditorRequired]
	public required Enums.ReleasesFilters Type { get; set; }

	[Parameter, EditorRequired]
	public required string Title { get; set; }

	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	private bool IsActive => IsFilterActive();
	private string ButtonClass => IsActive ? "active" : string.Empty;
	private string ButtonTitle => IsActive ? "Hide " + Title : "Show " + Title;

	protected override void OnInitialized()
	{
		SpotifyFilterService.OnFilterOrDataChanged += OnFilterOrDataChanged;
		base.OnInitialized();
	}

	public void Dispose()
	{
		SpotifyFilterService.OnFilterOrDataChanged -= OnFilterOrDataChanged;
	}

	private void OnFilterOrDataChanged()
	{
		InvokeAsync(StateHasChanged);
	}

	private async Task ChangeFilter()
	{
		var url = await SpotifyFilterUrlService.GetFilterUrl(Type, !IsActive);
		NavManager.NavigateTo(url);
	}

	private bool IsFilterActive()
	{
		// this names must be same as in the URL and in Enums.ReleasesFilters
		var filterProperty = SpotifyFilterService.Filter!.Advanced.GetType().GetProperty(Type.ToString());
		if (filterProperty is null || filterProperty.PropertyType != typeof(bool))
		{
			throw new NotSupportedException(nameof(filterProperty));
		}

		return (bool)filterProperty.GetValue(SpotifyFilterService.Filter.Advanced)!;
	}

}