using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.LoggedIn.Menus.Date;

public partial class ButtonDateMonth
{
	[Parameter, EditorRequired]
	public required int Year { get; set; }

	[Parameter, EditorRequired]
	public required int Month { get; set; }

	private string? ButtonClass => MonthFilter ? " active" : string.Empty;
	private bool MonthFilter => SpotifyFilterService.Filter.Month.HasValue && SpotifyFilterService.Filter.Month.Value.Year == Year && SpotifyFilterService.Filter.Month.Value.Month == Month;

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

	private void FilterMonth()
	{
		int? monthFilter = MonthFilter ? null : Month;
		int? yearFilter = MonthFilter ? null : Year;

		var url = SpotifyFilterUrlService.GetFilterUrl(yearFilter, monthFilter);
		NavManager.NavigateTo(url);
	}
}