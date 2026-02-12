using Microsoft.AspNetCore.Components;

namespace JakubKastner.MusicReleases.Web.Components.Base;

public partial class MrSearch
{
	[Parameter]
	public RenderFragment? ChildContent { get; set; }

	[Parameter]
	public string SearchTerm { get; set; } = string.Empty;

	[Parameter]
	public EventCallback<string> ValueChanged { get; set; }

	[Parameter]
	public string Placeholder { get; set; } = "Search...";

	[Parameter]
	public string? Class { get; set; }

	private async Task HandleInput(ChangeEventArgs e)
	{
		SearchTerm = e.Value?.ToString() ?? string.Empty;
		await ValueChanged.InvokeAsync(SearchTerm);
	}

	private async Task Clear()
	{
		SearchTerm = string.Empty;
		await ValueChanged.InvokeAsync(SearchTerm);
	}
}
